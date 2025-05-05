using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities;
using AttarStore.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttarStore.WebApi.Controllers
{

    [Route("api/admins/{adminId}/users")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public AdminUsersController(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        // GET /api/admins/1/users
        [HttpGet]
        [Authorize(Policy = "VendorUser.Read")]  // or a new Policy like "AdminUser.Read"
        public async Task<ActionResult<UserMapperView[]>> GetByAdmin(int adminId)
        {
            var users = await _userRepo.GetByAdminIdAsync(adminId);
            return Ok(_mapper.Map<UserMapperView[]>(users));
        }

        // POST /api/admins/1/users
        [HttpPost]
        [Authorize(Policy = "User.Create")] // or "AdminUser.Create"
        public async Task<IActionResult> Create(
            int adminId,
            [FromBody] UserMapperCreate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<User>(dto);
            user.AdminId = adminId;
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepo.AddAsync(user);
            var result = _mapper.Map<UserMapperView>(user);

            return CreatedAtAction(
                nameof(GetByAdmin),
                new { adminId },
                result
            );
        }
    }
}

