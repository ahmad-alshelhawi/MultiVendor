using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttarStore.WebApi.Controllers
{
    [Authorize(Policy = "Permission.Read")]
    [ApiController]
    [Route("api/[controller]")]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionRepository _rpRepo;
        private readonly IMapper _mapper;

        public RolePermissionController(IRolePermissionRepository rpRepo, IMapper mapper)
        {
            _rpRepo = rpRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(_mapper.Map<RolePermissionDto[]>(await _rpRepo.GetAllAsync()));

        [HttpPost]
        public async Task<IActionResult> Create(RolePermissionDto dto)
        {
            var entity = _mapper.Map<RolePermission>(dto);
            var created = await _rpRepo.AddAsync(entity);
            var result = _mapper.Map<RolePermissionDto>(created);
            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }


        [HttpDelete("{id}")]
        [Authorize(Policy = "Permission.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            await _rpRepo.DeleteAsync(id);
            return NoContent();
        }
    }

}
