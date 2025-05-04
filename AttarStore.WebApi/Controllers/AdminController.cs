using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttarStore.Application.Dtos;
using AttarStore.Domain.Interfaces;
using AttarStore.Domain.Entities;
using AttarStore.Domain.Entities.Auth;

namespace AttarStore.WebApi.Controllers

{
    [Authorize(Roles = Roles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepo;
        private readonly IUserRepository _userRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IMapper _mapper;

        public AdminController(
            IAdminRepository adminRepository,
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IMapper mapper)
        {
            _adminRepo = adminRepository;
            _userRepo = userRepository;
            _clientRepo = clientRepository;
            _mapper = mapper;
        }

        // GET: api/Admin
        [HttpGet]
        public async Task<ActionResult<AdminMapperView[]>> GetAll()
        {
            var admins = await _adminRepo.GetAllAsync();
            if (admins == null || !admins.Any())
                return NotFound("No admins found.");

            return Ok(_mapper.Map<AdminMapperView[]>(admins));
        }

        // POST: api/Admin
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminMapperCreate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // uniqueness checks
            if (await _adminRepo.GetByUsernameAsync(dto.Name) != null
             || await _userRepo.GetByNameAsync(dto.Name) != null
             || await _clientRepo.GetByNameAsync(dto.Name) != null)
            {
                return Conflict(new { status = "Name already exists in the system." });
            }

            if (await _adminRepo.GetByEmailAsync(dto.Email) != null
             || await _userRepo.GetByEmailAsync(dto.Email) != null
             || await _clientRepo.GetByEmailAsync(dto.Email) != null)
            {
                return Conflict(new { status = "Email already exists in the system." });
            }

            var admin = _mapper.Map<Admin>(dto);
            admin.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);

            await _adminRepo.AddAsync(admin);

            var resultDto = _mapper.Map<AdminMapperView>(admin);
            return CreatedAtAction(nameof(GetById), new { id = admin.Id }, resultDto);
        }

        // GET: api/Admin/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var admin = await _adminRepo.GetByIdAsync(id);
            if (admin == null)
                return NotFound(new { status = "Admin not found." });

            return Ok(_mapper.Map<AdminMapperView>(admin));
        }

        // PUT: api/Admin/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminMapperUpdate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _adminRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { status = "Admin not found." });

            // name uniqueness
            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != existing.Name)
            {
                if (await _adminRepo.GetByUsernameAsync(dto.Name) != null
                 || await _userRepo.GetByNameAsync(dto.Name) != null
                 || await _clientRepo.GetByNameAsync(dto.Name) != null)
                {
                    return Conflict(new { status = "Name already exists in the system." });
                }
            }

            // email uniqueness
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != existing.Email)
            {
                if (await _adminRepo.GetByEmailAsync(dto.Email) != null
                 || await _userRepo.GetByEmailAsync(dto.Email) != null
                 || await _clientRepo.GetByEmailAsync(dto.Email) != null)
                {
                    return Conflict(new { status = "Email already exists in the system." });
                }
            }

            _mapper.Map(dto, existing);
            await _adminRepo.UpdateAsync(existing);

            return Ok(_mapper.Map<AdminMapperView>(existing));
        }

        // DELETE: api/Admin/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _adminRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { status = "Admin not found." });

            await _adminRepo.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/Admin/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out int adminId))
                return Unauthorized(new { status = "Invalid token." });

            var admin = await _adminRepo.GetByIdAsync(adminId);
            if (admin == null)
                return NotFound(new { status = "Profile not found." });

            return Ok(new
            {
                admin.Name,
                admin.Email,
                admin.Phone,
                admin.Address
            });
        }

        // PUT: api/Admin/profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] AdminProfileUpdateMapper dto)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out int adminId))
                return Unauthorized(new { status = "Invalid token." });

            var result = await _adminRepo.UpdateProfileAsync(
                adminId, dto.Name, dto.Phone, dto.Email);

            if (result != "Profile updated successfully.")
                return Conflict(new { status = result });

            return Ok(new { status = result });
        }

        // POST: api/Admin/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordAdmin dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword)
             || string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest(new { status = "Current and new passwords are required." });
            }

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out int adminId))
                return Unauthorized(new { status = "Invalid token." });

            var success = await _adminRepo.UpdatePasswordAsync(
                adminId, dto.CurrentPassword, dto.NewPassword);

            if (!success)
                return BadRequest(new { status = "Password update failed." });

            return Ok(new { status = "Password changed successfully." });
        }
    }
}
