using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Services.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttarStore.WebApi.Controllers
{
    [Route("api/users/{userId}/permissions")]
    [ApiController]
    public class UserPermissionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public UserPermissionsController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "Permission.Read")]
        public async Task<IActionResult> Get(int userId)
        {
            var ups = await _db.UserPermissions
                              .Where(up => up.UserId == userId)
                              .ToListAsync();
            return Ok(_mapper.Map<UserPermissionDto[]>(ups));
        }

        [HttpPost]
        [Authorize(Policy = "Permission.Update")]
        public async Task<IActionResult> Upsert(int userId, UserPermissionDto dto)
        {
            var up = await _db.UserPermissions
                              .SingleOrDefaultAsync(x => x.UserId == userId && x.PermissionName == dto.PermissionName);

            if (up == null)
            {
                up = new UserPermission
                {
                    UserId = userId,
                    PermissionName = dto.PermissionName,
                    IsGranted = dto.IsGranted
                };
                _db.UserPermissions.Add(up);
            }
            else
            {
                up.IsGranted = dto.IsGranted;
                _db.UserPermissions.Update(up);
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{permissionName}")]
        [Authorize(Policy = "Permission.Delete")]
        public async Task<IActionResult> Delete(int userId, string permissionName)
        {
            var up = await _db.UserPermissions
                              .SingleOrDefaultAsync(x => x.UserId == userId && x.PermissionName == permissionName);
            if (up != null)
            {
                _db.UserPermissions.Remove(up);
                await _db.SaveChangesAsync();
            }
            return NoContent();
        }
    }

}
