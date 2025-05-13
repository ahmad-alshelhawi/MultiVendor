// WebApi/Controllers/NotificationController.cs
using AttarStore.Application.Dtos;
using AttarStore.Application.Interfaces;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AttarStore.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _svc;
    public NotificationController(INotificationService svc) => _svc = svc;

    [HttpPost("user/{userId}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ToUser(int userId, [FromBody] CreateNotificationDto dto)
    {
        await _svc.SendToUserAsync(userId, dto.Title, dto.Message);
        return NoContent();
    }

    [HttpPost("admin/{adminId}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ToAdmin(int adminId, [FromBody] CreateNotificationDto dto)
    {
        await _svc.SendToAdminAsync(adminId, dto.Title, dto.Message);
        return NoContent();
    }

    [HttpPost("client/{clientId}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ToClient(int clientId, [FromBody] CreateNotificationDto dto)
    {
        await _svc.SendToClientAsync(clientId, dto.Title, dto.Message);
        return NoContent();
    }

    [HttpPost("vendor/{vendorId}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ToVendor(int vendorId, [FromBody] CreateNotificationDto dto)
    {
        await _svc.SendToVendorStoreAsync(vendorId, dto.Title, dto.Message);
        return NoContent();
    }

    [HttpPost("role/{roleName}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ToUserRole(string roleName, [FromBody] CreateNotificationDto dto)
    {
        if (roleName != Roles.AdminUser
         && roleName != Roles.VendorAdmin
         && roleName != Roles.VendorUser)
            return BadRequest("Invalid role");
        await _svc.SendToUserRoleAsync(roleName, dto.Title, dto.Message);
        return NoContent();
    }

    [HttpPost("broadcast")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Broadcast([FromBody] CreateNotificationDto dto)
    {
        await _svc.BroadcastAsync(dto.Title, dto.Message);
        return NoContent();
    }

    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> Mine()
    {
        var uid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var inbox = await _svc.GetInboxAsync(uid);
        return Ok(inbox);
    }

    [HttpGet("all")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> All()
    {
        var all = await _svc.GetAllAsync();
        return Ok(all);
    }

    [HttpPut("read/{notificationId}")]
    [Authorize]
    public async Task<IActionResult> MarkRead(int notificationId)
    {
        var uid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _svc.MarkAsReadAsync(uid, notificationId);
        return NoContent();
    }

}
