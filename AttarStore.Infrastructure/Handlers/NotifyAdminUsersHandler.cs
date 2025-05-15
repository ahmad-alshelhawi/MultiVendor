using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AttarStore.Infrastructure.Events;
using AttarStore.Infrastructure.Interfaces;
using AttarStore.Infrastructure.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Handlers
{
    public class NotifyAdminUsersHandler : INotificationHandler<OrderPlacedEvent>
    {
        private readonly INotificationService _push;
        private readonly IEmailSender _email;
        private readonly IUserRepository _users;

        public NotifyAdminUsersHandler(
            INotificationService push,
            IEmailSender email,
            IUserRepository users)
        {
            _push = push;
            _email = email;
            _users = users;
        }

        public async Task Handle(OrderPlacedEvent evt, CancellationToken ct)
        {
            var admins = await _users.GetByRoleAsync(Roles.AdminUser);
            foreach (var a in admins)
            {
                // Real-time
                await _push.SendToUserRoleAsync(
                    Roles.AdminUser,
                    "Order Alert",
                    $"Order #{evt.OrderId} has been placed by client #{evt.ClientId}.");

                // Email
                var subject = $"Order #{evt.OrderId} Notification";
                var body = $"<p>Admin User,<br/>Order <b>#{evt.OrderId}</b> has been placed.</p>";
                await _email.SendEmailAsync(a.Email, subject, body);
            }
        }
    }
}
