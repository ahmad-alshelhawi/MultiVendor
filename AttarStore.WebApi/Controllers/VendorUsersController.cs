using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities;
using AttarStore.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttarStore.WebApi.Controllers
{


    [Route("api/vendors/{vendorId}/users")]
    [ApiController]
    public class VendorUsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public VendorUsersController(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        // GET /api/vendors/5/users
        [HttpGet]
        [Authorize(Policy = "VendorUser.Read")]
        public async Task<ActionResult<UserMapperView[]>> GetByVendor(int vendorId)
        {
            var users = await _userRepo.GetByVendorIdAsync(vendorId);
            return Ok(_mapper.Map<UserMapperView[]>(users));
        }

        // POST /api/vendors/5/users
        [HttpPost]
        [Authorize(Policy = "VendorUser.Create")]
        public async Task<IActionResult> Create(
            int vendorId,
            [FromBody] UserMapperCreate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<User>(dto);
            user.VendorId = vendorId;
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _userRepo.AddAsync(user);
            var result = _mapper.Map<UserMapperView>(user);

            return CreatedAtAction(
                nameof(GetByVendor),
                new { vendorId },
                result
            );
        }
    }
}

