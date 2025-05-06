using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AttarStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly IVendorRepository _vendorRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public VendorController(
            IVendorRepository vendorRepository,
            IUserRepository userRepository,
            IMapper mapper,
            IVendorService vendorService)
        {
            _vendorRepo = vendorRepository;
            _userRepo = userRepository;
            _mapper = mapper;
            _vendorService = vendorService;
        }

        // ─── Admin‐only management ────────────────────────────────────────────

        // GET: api/Vendor
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<VendorMapperView[]>> GetAll()
        {
            var vendors = await _vendorRepo.GetAllAsync();
            return Ok(_mapper.Map<VendorMapperView[]>(vendors));
        }

        // GET: api/Vendor/{id}
        [HttpGet("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<VendorMapperView>> GetById(int id)
        {
            var vendor = await _vendorRepo.GetByIdAsync(id);
            if (vendor == null)
                return NotFound(new { status = "Vendor not found." });
            return Ok(_mapper.Map<VendorMapperView>(vendor));
        }

        // POST: api/Vendor
        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] VendorMapperCreate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _vendorRepo.ExistsByNameOrContactEmailAsync(dto.Name, dto.ContactEmail))
                return Conflict(new { status = "Name or contact email already in use." });

            var vendor = _mapper.Map<Vendor>(dto);
            await _vendorRepo.AddAsync(vendor);

            return CreatedAtAction(nameof(GetById),
                new { id = vendor.Id },
                _mapper.Map<VendorMapperView>(vendor));
        }

        // PUT: api/Vendor/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] VendorMapperUpdate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _vendorRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { status = "Vendor not found." });

            if (await _vendorRepo.ExistsByNameOrContactEmailAsync(
                    dto.Name ?? existing.Name,
                    dto.ContactEmail ?? existing.ContactEmail,
                    excludeVendorId: id))
            {
                return Conflict(new { status = "Name or contact email already in use." });
            }

            _mapper.Map(dto, existing);
            await _vendorRepo.UpdateAsync(existing);

            return Ok(_mapper.Map<VendorMapperView>(existing));
        }

        // DELETE: api/Vendor/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _vendorRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { status = "Vendor not found." });

            await _vendorRepo.DeleteAsync(id);
            return NoContent();
        }

        // ─── Vendor‐self endpoints ────────────────────────────────────────────

        // GET: api/Vendor/profile
        [HttpGet("profile")]
        [Authorize(Roles = Roles.VendorAdmin + "," + Roles.VendorUser)]
        public async Task<IActionResult> GetProfile()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out int userId))
                return Unauthorized(new { status = "Invalid token." });

            var user = await _userRepo.GetByIdAsync(userId);
            if (user?.VendorId == null)
                return NotFound(new { status = "Vendor not found for this user." });

            var vendor = await _vendorRepo.GetByIdAsync(user.VendorId.Value);
            if (vendor == null)
                return NotFound(new { status = "Vendor not found." });

            return Ok(new
            {
                vendor.Name,
                vendor.ContactEmail,
                vendor.Phone,
                vendor.Address
            });
        }

        // PUT: api/Vendor/profile
        [HttpPut("profile")]
        [Authorize(Roles = Roles.VendorAdmin)]
        public async Task<IActionResult> UpdateProfile([FromBody] VendorProfileUpdateMapper dto)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out int userId))
                return Unauthorized(new { status = "Invalid token." });

            var user = await _userRepo.GetByIdAsync(userId);
            if (user?.VendorId == null)
                return NotFound(new { status = "Vendor not found for this user." });

            var existing = await _vendorRepo.GetByIdAsync(user.VendorId.Value);
            if (existing == null)
                return NotFound(new { status = "Vendor not found." });

            if (await _vendorRepo.ExistsByNameOrContactEmailAsync(
                    dto.Name ?? existing.Name,
                    dto.ContactEmail ?? existing.ContactEmail,
                    excludeVendorId: existing.Id))
            {
                return Conflict(new { status = "Name or contact email already in use." });
            }

            _mapper.Map(dto, existing);
            await _vendorRepo.UpdateAsync(existing);

            return Ok(new { status = "Profile updated successfully." });
        }



        [HttpPost("{id}/suspend")]
        [Authorize(Policy = "Vendor.Update")]
        public async Task<IActionResult> Suspend(int id)
        {
            await _vendorService.SuspendVendorAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/activate")]
        [Authorize(Policy = "Vendor.Update")]
        public async Task<IActionResult> Activate(int id)
        {
            await _vendorService.ActivateVendorAsync(id);
            return NoContent();
        }
    }
}
