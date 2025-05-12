using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace AttarStore.Infrastructure.Hubs
{
    //[Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                // join user-specific group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

                // join all role groups
                var roles = Context.User.Claims
                               .Where(c => c.Type == ClaimTypes.Role)
                               .Select(c => c.Value);
                foreach (var r in roles)
                    await Groups.AddToGroupAsync(Context.ConnectionId, r);
            }

            await base.OnConnectedAsync();
        }
    }
}
