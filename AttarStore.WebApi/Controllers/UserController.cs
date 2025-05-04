using System.Security.Claims;
using System.Threading.Tasks;
using AttarStore.Application.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttarStore.Domain.Entities;
using AttarStore.Domain.Interfaces;
using AttarStore.Domain.Entities.Auth;

namespace AttarStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IAdminRepository _adminRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IMapper _mapper;

        public UserController(
            IUserRepository userRepository,
            IAdminRepository adminRepository,
            IClientRepository clientRepository,
            IMapper mapper)
        {
            _userRepo = userRepository;
            _adminRepo = adminRepository;
            _clientRepo = clientRepository;
            _mapper = mapper;
        }

        // ─── Admin‐only management endpoints ────────────────────────────────────

        // GET: api/User
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<UserMapperView[]>> GetAll()
        {
            var users = await _userRepo.GetAllAsync();
            return Ok(_mapper.Map<UserMapperView[]>(users));
        }

        // GET: api/User/{id}
        [HttpGet("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<UserMapperView>> GetById(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { status = "User not found." });
            return Ok(_mapper.Map<UserMapperView>(user));
        }

        // POST: api/User
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] UserMapperCreate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _userRepo.ExistsByNameOrEmailAsync(dto.Name, dto.Email))
                return Conflict(new { status = "Name or email already exists." });

            var user = _mapper.Map<User>(dto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            await _userRepo.AddAsync(user);

            return CreatedAtAction(nameof(GetById), new { id = user.Id },
                _mapper.Map<UserMapperView>(user));
        }

        // PUT: api/User/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UserMapperUpdate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _userRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { status = "User not found." });

            if (await _userRepo.ExistsByNameOrEmailAsync(
                    dto.Name ?? existing.Name,
                    dto.Email ?? existing.Email,
                    excludeUserId: id))
            {
                return Conflict(new { status = "Name or email already exists." });
            }

            _mapper.Map(dto, existing);
            await _userRepo.UpdateAsync(existing);

            return Ok(_mapper.Map<UserMapperView>(existing));
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _userRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { status = "User not found." });

            await _userRepo.DeleteAsync(id);
            return NoContent();
        }

        // ─── Vendor‐user self‐service endpoints ──────────────────────────────────

        // GET: api/User/profile
        [HttpGet("profile")]
        [Authorize(Roles = Roles.VendorAdmin + "," + Roles.VendorUser)]
        public async Task<IActionResult> GetProfile()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out int userId))
                return Unauthorized(new { status = "Invalid token." });

            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { status = "Profile not found." });

            return Ok(new
            {
                user.Name,
                user.Email,
                user.Phone,
                user.Address
            });
        }

        // PUT: api/User/profile
        [HttpPut("profile")]
        [Authorize(Roles = Roles.VendorAdmin + "," + Roles.VendorUser)]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateMapper dto)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out int userId))
                return Unauthorized(new { status = "Invalid token." });

            var result = await _userRepo.UpdateProfileAsync(
                userId, dto.Name, dto.Phone, dto.Email);

            if (result != "Profile updated successfully.")
                return Conflict(new { status = result });

            return Ok(new { status = result });
        }

        // POST: api/User/change-password
        [HttpPost("change-password")]
        [Authorize(Roles = Roles.VendorAdmin + "," + Roles.VendorUser)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordUser dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
                string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest(new { status = "Current and new passwords are required." });
            }

            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out int userId))
                return Unauthorized(new { status = "Invalid token." });

            var success = await _userRepo.UpdatePasswordAsync(
                userId, dto.CurrentPassword, dto.NewPassword);

            if (!success)
                return BadRequest(new { status = "Password update failed." });

            return Ok(new { status = "Password changed successfully." });
        }
    }
}
