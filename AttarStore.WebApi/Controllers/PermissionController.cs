using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttarStore.WebApi.Controllers
{
    [Authorize(Policy = "Permission.Read")]  // only admins
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _permRepo;
        private readonly IRolePermissionRepository _rpRepo;
        private readonly IMapper _mapper;

        public PermissionController(
            IPermissionRepository permRepo,
            IRolePermissionRepository rpRepo,
            IMapper mapper)
        {
            _permRepo = permRepo;
            _rpRepo = rpRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var perms = await _permRepo.GetAllAsync();
            return Ok(_mapper.Map<PermissionDto[]>(perms));
        }

        [HttpPost]
        [Authorize(Policy = "Permission.Update")]
        public async Task<IActionResult> Create([FromBody] PermissionDto dto)
        {
            var entity = _mapper.Map<Permission>(dto);
            await _permRepo.AddAsync(entity);
            return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, _mapper.Map<PermissionDto>(entity));
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Permission.Update")]
        public async Task<IActionResult> Update(int id, [FromBody] PermissionDto dto)
        {
            var existing = await _permRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            _mapper.Map(dto, existing);
            await _permRepo.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Permission.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _permRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
