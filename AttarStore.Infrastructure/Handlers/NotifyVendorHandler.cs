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
    public class NotifyVendorHandler : INotificationHandler<OrderPlacedEvent>
    {
        private readonly INotificationService _push;
        private readonly IEmailSender _email;
        private readonly IVendorRepository _vendors;

        public NotifyVendorHandler(
            INotificationService push,
            IEmailSender email,
            IVendorRepository vendors)
        {
            _push = push;
            _email = email;
            _vendors = vendors;
        }

        public async Task Handle(OrderPlacedEvent evt, CancellationToken ct)
        {
            foreach (var vid in evt.VendorIds)
            {
                // Real-time
                await _push.SendToVendorStoreAsync(
                    vid,
                    "New Order Received",
                    $"Order #{evt.OrderId} has been placed for your store.");

                // Email
                var vendor = await _vendors.GetByIdAsync(vid);
                var subject = $"New Order #{evt.OrderId}";
                var body = $"<p>Hi {vendor.Name},<br/>You have a new order <b>#{evt.OrderId}</b> pending.</p>";
                await _email.SendEmailAsync(vendor.ContactEmail, subject, body);
            }
        }
    }
}
