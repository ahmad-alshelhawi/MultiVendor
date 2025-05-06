using AttarStore.Domain.Entities.Shopping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttarStore.Domain.Entities.Catalog
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public int? SubcategoryId { get; set; }
        public Subcategory Subcategory { get; set; }
        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; }

        // NEW: default price/stock if *no* variants
        public decimal DefaultPrice { get; set; }
        public int DefaultStock { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
       = new List<InventoryTransaction>();
    }

}
