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
    public class NotifyClientHandler : INotificationHandler<OrderPlacedEvent>
    {
        private readonly INotificationService _push;
        private readonly IEmailSender _email;
        private readonly IClientRepository _clients;

        public NotifyClientHandler(
            INotificationService push,
            IEmailSender email,
            IClientRepository clients)
        {
            _push = push;
            _email = email;
            _clients = clients;
        }

        public async Task Handle(OrderPlacedEvent evt, CancellationToken ct)
        {
            // Real-time in-app
            await _push.SendToClientAsync(
                evt.ClientId,
                "Your Order Is Confirmed",
                $"Your order #{evt.OrderId} has been placed successfully.");

            // Email
            var client = await _clients.GetByIdAsync(evt.ClientId);
            var subject = $"Order #{evt.OrderId} Confirmation";
            var body = $"<p>Hi {client.Name},<br/>Thank you for your purchase! Your order <b>#{evt.OrderId}</b> is confirmed.</p>";
            await _email.SendEmailAsync(client.Email, subject, body);
        }
    }
}
