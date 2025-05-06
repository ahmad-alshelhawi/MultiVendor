using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.Dtos.Shopping
{

    public class InventoryTransactionCreateDto
    {
        /// <summary>Either ProductVariantId (preferred) or ProductId for non‐variant stock.</summary>
        public int? ProductVariantId { get; set; }

        public int? ProductId { get; set; }

        [Required]
        public int QuantityChange { get; set; }

        [Required]
        public string Reason { get; set; }

        public DateTime? Timestamp { get; set; }  // optional: will be set to UTC Now if null
    }

    public class InventoryTransactionDto
    {
        public int Id { get; set; }
        public int? ProductVariantId { get; set; }
        public int? ProductId { get; set; }
        public int QuantityChange { get; set; }
        public string Reason { get; set; }
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }
}
