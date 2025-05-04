using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AttarStore.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IEmailSender = AttarStore.Infrastructure.Services.IEmailSender;
using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities;
using AttarStore.Domain.Entities.submodels;
using AttarStore.Domain.Interfaces;
using AttarStore.Infrastructure.Services;
using AttarStore.Domain.Entities.Auth;

namespace AttarStore.WebApi.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IAdminRepository _adminRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IRefreshTokenRepository _rtRepo;
        private readonly TokenService _tokenService;
        private readonly IEmailSender _emailSender;

        public AuthController(
            IUserRepository userRepo,
            IAdminRepository adminRepo,
            IClientRepository clientRepo,
            IRefreshTokenRepository rtRepo,
            TokenService tokenService,
            IEmailSender emailSender)
        {
            _userRepo = userRepo;
            _adminRepo = adminRepo;
            _clientRepo = clientRepo;
            _rtRepo = rtRepo;
            _tokenService = tokenService;
            _emailSender = emailSender;
        }

        // ─── LOGIN ─────────────────────────────────────────────────────────────────
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel creds)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IUser foundUser = null;
            string role = null;

            // 1) Try Admin by username or email
            var admin = await _adminRepo.GetByUsernameAsync(creds.Name)
                       ?? await _adminRepo.GetByEmailAsync(creds.Name);
            if (admin != null && BCrypt.Net.BCrypt.Verify(creds.Password, admin.Password))
            {
                foundUser = admin;
                role = Roles.Admin;
            }
            else
            {
                // 2) Try regular User
                var user = await _userRepo.GetByUsernameOrEmailAsync(creds.Name)
                         as User;
                if (user != null && BCrypt.Net.BCrypt.Verify(creds.Password, user.Password))
                {
                    foundUser = user;
                    role = user.Role;  // e.g. Roles.VendorAdmin or Roles.VendorUser or Roles.Customer
                }
                else
                {
                    // 3) Try Client
                    var client = await _clientRepo.GetByUsernameOrEmailAsync(creds.Name)
                               as Client;
                    if (client != null && BCrypt.Net.BCrypt.Verify(creds.Password, client.Password))
                    {
                        foundUser = client;
                        role = Roles.Client;
                    }
                }
            }

            if (foundUser == null)
                return Unauthorized(new { message = "Invalid credentials." });

            // 4) Determine which ID slot to use
            int? userId = foundUser is User ? foundUser.Id : null;
            int? adminId = foundUser is Admin ? foundUser.Id : null;
            int? clientId = foundUser is Client ? foundUser.Id : null;

            // 5) Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(
                                   foundUser.Id.ToString(),
                                   foundUser.Name,
                                   role);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // 6) Persist refresh token
            await _rtRepo.CreateAsync(new RefreshToken
            {
                Token = refreshToken,
                Role = role,
                UserId = userId,
                AdminId = adminId,
                ClientId = clientId,
                ExpiryDate = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenValidityDays),
                IsRevoked = false
            });

            // 7) Set cookies
            var rtOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenValidityDays)
            };
            Response.Cookies.Append("refreshToken", refreshToken, rtOptions);

            var atOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(_tokenService.AccessTokenValidityMinutes)
            };
            Response.Cookies.Append("accessToken", accessToken, atOptions);

            return Ok(new { expiresIn = _tokenService.AccessTokenValidityMinutes * 60 });
        }

        // ─── REFRESH ───────────────────────────────────────────────────────────────
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var oldToken))
                return Unauthorized();

            var stored = await _rtRepo.GetByTokenAsync(oldToken);
            if (stored == null || stored.IsRevoked || stored.ExpiryDate <= DateTime.UtcNow)
                return Unauthorized();

            // Revoke old
            stored.IsRevoked = true;
            await _rtRepo.UpdateAsync(stored);

            // Create new
            var newRt = _tokenService.GenerateRefreshToken();
            var newEntity = new RefreshToken
            {
                Token = newRt,
                Role = stored.Role,
                UserId = stored.UserId,
                AdminId = stored.AdminId,
                ClientId = stored.ClientId,
                ExpiryDate = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenValidityDays),
                IsRevoked = false
            };
            await _rtRepo.CreateAsync(newEntity);

            var newAt = _tokenService.GenerateAccessToken(
                (stored.AdminId ?? stored.UserId ?? stored.ClientId).ToString(),
                "",
                stored.Role);

            // Set cookies
            Response.Cookies.Append("refreshToken", newRt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = newEntity.ExpiryDate
            });
            Response.Cookies.Append("accessToken", newAt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddMinutes(_tokenService.AccessTokenValidityMinutes)
            });

            return Ok(new { expiresIn = _tokenService.AccessTokenValidityMinutes * 60 });
        }

        // ─── WHO AM I ─────────────────────────────────────────────────────────────
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (!int.TryParse(idClaim, out var userId) || string.IsNullOrEmpty(role))
                return Unauthorized();

            object profile = role switch
            {
                Roles.Admin => (object)await _adminRepo.GetByIdAsync(userId),
                Roles.Client => await _clientRepo.GetByIdAsync(userId),
                _ => await _userRepo.GetByIdAsync(userId)
            };

            return Ok(new
            {
                id = userId,
                name = ((IUser)profile).Name,
                email = ((IUser)profile).Email,
                role
            });
        }

        // ─── LOGOUT ───────────────────────────────────────────────────────────────
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("refreshToken", out var rtValue))
            {
                var rt = await _rtRepo.GetByTokenAsync(rtValue);
                if (rt != null)
                {
                    rt.IsRevoked = true;
                    await _rtRepo.UpdateAsync(rt);
                }
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            };
            Response.Cookies.Delete("refreshToken", cookieOptions);
            Response.Cookies.Delete("accessToken", cookieOptions);

            return NoContent();
        }

        // ─── PASSWORD RESET REQUEST ──────────────────────────────────────────────
        [HttpPost("request-password-reset")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest(new { message = "Email is required." });

            // Try Admin, then Client, then User
            if (await _adminRepo.GetByEmailAsync(model.Email) is Admin)
            {
                var token = await _adminRepo.GenerateResetTokenAsync(model.Email);
                if (token == null) return BadRequest(new { message = "Error generating reset token." });
                var link = $"{AppConstants.PasswordResetBaseUrl}?token={token}&email={model.Email}";
                await _emailSender.SendEmailAsync(model.Email, "Password Reset", $"Click to reset: {link}");
                return Ok(new { message = "Password reset link sent for Admin." });
            }
            if (await _clientRepo.GetByEmailAsync(model.Email) is Client)
            {
                var token = await _clientRepo.GenerateResetTokenAsync(model.Email);
                if (token == null) return BadRequest(new { message = "Error generating reset token." });
                var link = $"{AppConstants.PasswordResetBaseUrl}?token={token}&email={model.Email}";
                await _emailSender.SendEmailAsync(model.Email, "Password Reset", $"Click to reset: {link}");
                return Ok(new { message = "Password reset link sent for Client." });
            }
            if (await _userRepo.GetByEmailAsync(model.Email) is User)
            {
                var token = await _userRepo.GenerateResetTokenAsync(model.Email);
                if (token == null) return BadRequest(new { message = "Error generating reset token." });
                var link = $"{AppConstants.PasswordResetBaseUrl}?token={token}&email={model.Email}";
                await _emailSender.SendEmailAsync(model.Email, "Password Reset", $"Click to reset: {link}");
                return Ok(new { message = "Password reset link sent for User." });
            }

            return BadRequest(new { message = "Invalid email address." });
        }

        // ─── RESET PASSWORD ───────────────────────────────────────────────────────
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (string.IsNullOrEmpty(model.Email)
             || string.IsNullOrEmpty(model.Token)
             || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest(new { message = "Email, token, and new password are required." });
            }

            // Try Admin, then Client, then User
            if (await _adminRepo.ResetPasswordAsync(model.Email, model.Token, model.NewPassword))
                return Ok(new { message = "Password reset successful for Admin." });

            if (await _clientRepo.ResetPasswordAsync(model.Email, model.Token, model.NewPassword))
                return Ok(new { message = "Password reset successful for Client." });

            if (await _userRepo.ResetPasswordAsync(model.Email, model.Token, model.NewPassword))
                return Ok(new { message = "Password reset successful for User." });

            return BadRequest(new { message = "Invalid or expired token." });
        }
    }
}
