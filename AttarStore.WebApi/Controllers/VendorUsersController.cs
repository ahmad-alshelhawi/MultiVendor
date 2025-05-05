using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    // GET /api/vendors/{vendorId}/users
    [HttpGet]
    [Authorize(Roles = "Admin")]
/*    [Authorize(Policy = "VendorUser.Read")]
*/    public async Task<ActionResult<UserMapperView[]>> GetByVendor(int vendorId)
    {
        var users = await _userRepo.GetByVendorIdAsync(vendorId);
        return Ok(_mapper.Map<UserMapperView[]>(users));
    }

    // POST /api/vendors/{vendorId}/users
    [HttpPost]
    [Authorize(Roles = "Admin")]
/*    [Authorize(Policy = "VendorUser.Read")]
*/    public async Task<IActionResult> Create(
        int vendorId,
        [FromBody] VendorUserCreate dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // 1) Map incoming DTO
        var user = _mapper.Map<User>(dto);

        // 2) Force vendor scope and role
        user.VendorId = vendorId;
        user.Role = Roles.VendorUser;

        // 3) Hash password
        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // 4) Persist
        await _userRepo.AddAsync(user);

        // 5) Return the created user
        var result = _mapper.Map<UserMapperView>(user);
        return CreatedAtAction(
            nameof(GetByVendor),
            new { vendorId },
            result
        );
    }
}