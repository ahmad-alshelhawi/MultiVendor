using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.Dtos.Shopping
{
    // ─── Cart View ────────────────────────────────────────────────────────────
    public class CartView
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<CartItemView> Items { get; set; }
    }

    // ─── Cart‐Item View ────────────────────────────────────────────────────────
    public class CartItemView
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    // ─── Add Item DTO ─────────────────────────────────────────────────────────
    public class CartItemCreate
    {
        [Required] public int ProductVariantId { get; set; }
        [Required] public int Quantity { get; set; }
    }

    // ─── Update Item DTO ──────────────────────────────────────────────────────
    public class CartItemUpdate
    {
        [Required] public int Quantity { get; set; }
    }
}
