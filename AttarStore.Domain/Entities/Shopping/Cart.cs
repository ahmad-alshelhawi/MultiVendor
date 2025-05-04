using System;
using System.Collections.Generic;
using AttarStore.Domain.Entities;

namespace AttarStore.Domain.Entities.Shopping
{
    public class Cart
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }

        // ── UPDATED: remove dynamic initializer
        public DateTimeOffset CreatedAt { get; set; }

        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
