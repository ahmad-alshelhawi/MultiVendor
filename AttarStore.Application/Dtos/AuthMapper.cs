using System.ComponentModel.DataAnnotations;

namespace AttarStore.Application.Dtos
{
    // ─── LOGIN ───────────────────────────────────────────────────────────────
    public class LoginModel
    {
        [Required] public string Name { get; set; }
        [Required] public string Password { get; set; }
    }

    // ─── TOKEN RESPONSE ──────────────────────────────────────────────────────
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }  // seconds
    }

    // ─── REFRESH REQUEST (if you want a body‐based refresh) ──────────────────
    public class TokenRequest
    {
        [Required] public string RefreshToken { get; set; }
    }

    // ─── PASSWORD RESET REQUEST ─────────────────────────────────────────────
    public class PasswordResetRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }

    // ─── RESET PASSWORD ───────────────────────────────────────────────────────
    public class ResetPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required] public string Token { get; set; }
        [Required] public string NewPassword { get; set; }
    }
}
