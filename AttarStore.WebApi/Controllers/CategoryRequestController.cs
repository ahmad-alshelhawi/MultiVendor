using AttarStore.Application.Dtos.Catalog;
using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttarStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryRequestController : ControllerBase
    {
        private readonly ICategoryRequestRepository _repo;
        private readonly IMapper _mapper;

        public CategoryRequestController(
            ICategoryRequestRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Policy = "CategoryRequest.Create")]
        public async Task<ActionResult<CategoryRequestDto>> Create([FromBody] CategoryRequestCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vendorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(vendorIdClaim, out var vendorId))
                return Unauthorized();

            var entity = _mapper.Map<CategoryRequest>(dto);
            entity.VendorId = vendorId;

            // This returns void, so just await it
            await _repo.AddAsync(entity);

            // The entity now has an ID (EF will have set it)
            var result = _mapper.Map<CategoryRequestDto>(entity);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }


        /// <summary>
        /// Vendor reads their own requests
        /// </summary>
        [HttpGet("my")]
        [Authorize(Policy = "CategoryRequest.ReadOwn")]
        public async Task<ActionResult<CategoryRequestDto[]>> GetMine()
        {
            var vendorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(vendorIdClaim, out var vendorId))
                return Unauthorized();

            var list = await _repo.GetByVendorAsync(vendorId);
            return Ok(_mapper.Map<CategoryRequestDto[]>(list));
        }

        /// <summary>
        /// Admin reads all requests
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "CategoryRequest.ReadAll")]
        public async Task<ActionResult<CategoryRequestDto[]>> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(_mapper.Map<CategoryRequestDto[]>(list));
        }

        /// <summary>
        /// Get a single request by ID (vendor or admin)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "CategoryRequest.ReadOwn")]            // same policy works for admin too
        public async Task<ActionResult<CategoryRequestDto>> GetById(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            // if vendor, ensure ownership
            if (User.IsInRole("Vendor") && item.VendorId != int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                return Forbid();

            return Ok(_mapper.Map<CategoryRequestDto>(item));
        }

        /// <summary>
        /// Admin approves or rejects a request
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "CategoryRequest.Update")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] CategoryRequestUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            _mapper.Map(dto, existing);
            await _repo.UpdateAsync(existing);
            return NoContent();
        }
    }
}
