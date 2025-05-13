using AttarStore.Domain.Entities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace AttarStore.Infrastructure.Hubs
{

    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var idClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null) return;
            var id = idClaim.Value;

            // Admins (core Admin table)
            if (Context.User.IsInRole(Roles.Admin))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Admin_{id}");

            // Clients (core Client table)
            if (Context.User.IsInRole(Roles.Client))
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Client_{id}");

            // User-table roles only
            if (Context.User.IsInRole(Roles.AdminUser)
             || Context.User.IsInRole(Roles.VendorAdmin)
             || Context.User.IsInRole(Roles.VendorUser))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{id}");
            }

            await base.OnConnectedAsync();
        }
    }
}


