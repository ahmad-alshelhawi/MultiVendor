using AttarStore.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Events
{
    public class OrderPlacedEvent : INotification
    {
        public int OrderId { get; }
        public int ClientId { get; }
        public IReadOnlyList<int> VendorIds { get; }

        public OrderPlacedEvent(int orderId, int clientId, IEnumerable<int> vendorIds)
        {
            OrderId = orderId;
            ClientId = clientId;
            VendorIds = vendorIds.Distinct().ToList();
        }
    }
}
