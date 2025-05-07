// Domain/Entities/Shopping/InventoryTransaction.cs  (new)

using AttarStore.Domain.Entities.Catalog;
using System;
using System.ComponentModel.DataAnnotations;

namespace AttarStore.Domain.Entities.Shopping
{

    public class InventoryTransaction
    {
        public int Id { get; set; }

        // Either variant or base-product
        public int? ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }

        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }
        public string Reason { get; set; } = "";

        public DateTime Timestamp { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }
    }

}
