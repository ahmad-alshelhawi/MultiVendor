// Domain/Entities/Shopping/InventoryTransaction.cs  (new)

using AttarStore.Domain.Entities.Catalog;
using System;
using System.ComponentModel.DataAnnotations;

namespace AttarStore.Domain.Entities.Shopping
{

    public class InventoryTransaction
    {
        public int Id { get; set; }

        /// <summary>
        /// If this transaction is against a variant, set this.
        /// </summary>
        public int? ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }

        /// <summary>
        /// If this transaction is against a non-variant product, set this.
        /// </summary>
        public int? ProductId { get; set; }
        public Product Product { get; set; }

        /// <summary>
        /// How many items were added (positive) or removed (negative).
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Why the stock changed (e.g. "OrderPlaced", "ManualAdjustment", etc.).
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// When the change happened.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Which user performed the change.
        /// </summary>
        public int? UserId { get; set; }
        public User User { get; set; }
    }

}
