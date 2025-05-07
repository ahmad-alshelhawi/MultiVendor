using AttarStore.Domain.Entities;
using System;
using System.Collections.Generic;

namespace AttarStore.Domain.Entities.Shopping
{
    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }
        public DateTime CreatedAt { get; set; }

        // ← add this:
        public string Status { get; set; } = "Pending";

        public ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();
    }
}
