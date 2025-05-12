// WebApi/Controllers/NotificationController.cs
using AttarStore.Application.Dtos;
using AttarStore.Application.Interfaces;
using AttarStore.Domain.Interfaces;
using AttarStore.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _svc;
    public NotificationController(INotificationService svc) => _svc = svc;

    [HttpPost("user")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendToUser([FromBody] CreateNotificationDto dto)
    {
        var result = await _svc.CreateForUserAsync(dto);
        return CreatedAtAction(nameof(GetMine), new { }, result);
    }

    [HttpPost("role/{roleName}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendToRole(string roleName, [FromBody] CreateNotificationDto dto)
    {
        var list = await _svc.CreateForRoleAsync(dto, roleName);
        return Created("", list);
    }

    [HttpPost("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Broadcast([FromBody] CreateNotificationDto dto)
    {
        var list = await _svc.CreateForAllAsync(dto);
        return Created("", list);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var list = await _svc.GetUserNotificationsAsync(userId);
        return Ok(list);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _svc.MarkAsReadAsync(id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
