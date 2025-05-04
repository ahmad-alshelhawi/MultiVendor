using System;
using System.Collections.Generic;
using AttarStore.Domain.Entities;

namespace AttarStore.Domain.Entities.Shopping
{
    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }

        // ── UPDATED: remove dynamic initializer
        public DateTimeOffset CreatedAt { get; set; }

        public string Status { get; set; } = "Pending";
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
