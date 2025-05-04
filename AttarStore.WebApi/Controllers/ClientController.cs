using System.Security.Claims;
using System.Threading.Tasks;
using AttarStore.Application.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttarStore.Domain.Interfaces;
using AttarStore.Domain.Entities;
using AttarStore.Domain.Entities.Auth;

namespace AttarStore.WebApi.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientRepository _clientRepo;
        private readonly IUserRepository _userRepo;
        private readonly IAdminRepository _adminRepo;
        private readonly IMapper _mapper;

        public ClientController(
            IClientRepository clientRepository,
            IUserRepository userRepository,
            IAdminRepository adminRepository,
            IMapper mapper)
        {
            _clientRepo = clientRepository;
            _userRepo = userRepository;
            _adminRepo = adminRepository;
            _mapper = mapper;
        }

        // ─── Admin‐only management endpoints ────────────────────────────────────

        // GET: api/Client
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<ClientMapperView[]>> GetAll()
        {
            var clients = await _clientRepo.GetAllAsync();
            return Ok(_mapper.Map<ClientMapperView[]>(clients));
        }

        // GET: api/Client/{id}
        [HttpGet("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<ClientMapperView>> GetById(int id)
        {
            var client = await _clientRepo.GetByIdAsync(id);
            if (client == null)
                return NotFound(new { status = "Client not found." });

            return Ok(_mapper.Map<ClientMapperView>(client));
        }

        // POST: api/Client
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] ClientMapperCreate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check uniqueness
            if (await _clientRepo.ExistsByNameOrEmailAsync(dto.Name, dto.Email))
                return Conflict(new { status = "Name or email already exists." });

            var client = _mapper.Map<Client>(dto);
            client.Password = BCrypt.Net.BCrypt.HashPassword(client.Password);

            await _clientRepo.AddAsync(client);

            var view = _mapper.Map<ClientMapperView>(client);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, view);
        }

        // PUT: api/Client/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] ClientMapperUpdate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _clientRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { status = "Client not found." });

            // Check uniqueness excluding this client
            if (await _clientRepo.ExistsByNameOrEmailAsync(
                    dto.Name ?? existing.Name,
                    dto.Email ?? existing.Email,
                    excludeClientId: id))
            {
                return Conflict(new { status = "Name or email already exists." });
            }

            _mapper.Map(dto, existing);
            await _clientRepo.UpdateAsync(existing);

            return Ok(_mapper.Map<ClientMapperView>(existing));
        }

        // DELETE: api/Client/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _clientRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { status = "Client not found." });

            await _clientRepo.DeleteAsync(id);
            return NoContent();
        }

        // ─── Client‐only self‐service endpoints ───────────────────────────────────

        // GET: api/Client/profile
        [HttpGet("profile")]
        [Authorize(Roles = Roles.Client)]
        public async Task<IActionResult> GetProfile()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out int clientId))
                return Unauthorized(new { status = "Invalid token." });

            var client = await _clientRepo.GetByIdAsync(clientId);
            if (client == null)
                return NotFound(new { status = "Profile not found." });

            return Ok(new
            {
                client.Name,
                client.Email,
                client.Phone,
                client.Address
            });
        }

        // PUT: api/Client/profile
        [HttpPut("profile")]
        [Authorize(Roles = Roles.Client)]
        public async Task<IActionResult> UpdateProfile([FromBody] ClientProfileUpdateMapper dto)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out int clientId))
                return Unauthorized(new { status = "Invalid token." });

            var result = await _clientRepo.UpdateProfileAsync(
                clientId, dto.Name, dto.Phone, dto.Email);

            if (result != "Profile updated successfully.")
                return Conflict(new { status = result });

            return Ok(new { status = result });
        }

        // POST: api/Client/change-password
        [HttpPost("change-password")]
        [Authorize(Roles = Roles.Client)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordClient dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
                string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest(new { status = "Both current and new passwords are required." });
            }

            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out int clientId))
                return Unauthorized(new { status = "Invalid token." });

            var success = await _clientRepo.UpdatePasswordAsync(
                clientId, dto.CurrentPassword, dto.NewPassword);

            if (!success)
                return BadRequest(new { status = "Password update failed." });

            return Ok(new { status = "Password changed successfully." });
        }
    }
}
