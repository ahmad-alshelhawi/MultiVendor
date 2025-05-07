using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.Dtos.Shopping
{
    // ─── Order View ──────────────────────────────────────────────────────────
    /*public class OrderViewDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Status { get; set; } = null!;
        public DateTimeOffset Timestamp { get; set; }
        public IEnumerable<OrderItemView> Items { get; set; } = new List<OrderItemView>();
    }

    // ─── Order‐Item View ─────────────────────────────────────────────────────
    public class OrderItemView
    {
        public int ProductVariantId { get; set; }
        public string Sku { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    // ─── Create Order DTO ────────────────────────────────────────────────────
    public class OrderCreate
    {
        [Required]
        public List<OrderItemCreate> Items { get; set; }
    }

    public class OrderItemCreate
    {
        [Required] public int ProductVariantId { get; set; }
        [Required] public int Quantity { get; set; }
    }

    // ─── Update Status DTO ──────────────────────────────────────────────────
    public class OrderStatusUpdate
    {
        [Required] public string Status { get; set; }
    }*/


    public class OrderItemDto
    {
        public int ProductVariantId { get; set; }
        public string VariantSku { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public OrderItemDto[] Items { get; set; }
    }
}