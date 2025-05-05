using AttarStore.Application.Dtos.Catalog;
using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttarStore.WebApi.Controllers
{
    [Route("api/category-requests")]
    [ApiController]
    public class CategoryRequestsController : ControllerBase
    {
        private readonly ICategoryRequestRepository _repo;
        public CategoryRequestsController(ICategoryRequestRepository repo) => _repo = repo;

        // Vendor submits a new request
        [HttpPost]
        [Authorize(Roles = "VendorAdmin")]
        [Authorize(Policy = "CategoryRequest.Create")]
        public async Task<IActionResult> Post([FromBody] CategoryRequestCreateDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var req = new CategoryRequest
            {
                VendorId = userId,
                Name = dto.Name,
                Description = dto.Description
            };
            var created = await _repo.CreateAsync(req);
            return CreatedAtAction(null, new { id = created.Id }, created);
        }

        // Vendor views own requests
        [HttpGet("mine")]
        [Authorize(Roles = "VendorAdmin")]
        [Authorize(Policy = "CategoryRequest.ReadOwn")]
        public async Task<IActionResult> Mine()
          => Ok(await _repo.GetByVendorAsync(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)));

        // Admin views & approves
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        [Authorize(Policy = "CategoryRequest.ReadAll")]
        public async Task<IActionResult> Pending()
          => Ok(await _repo.GetAllPendingAsync());

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        [Authorize(Policy = "CategoryRequest.Update")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] CategoryRequestUpdateStatusDto dto)
          => Ok(await _repo.UpdateStatusAsync(id, dto.Status));
    }

}
