using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Entities.Catalog
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string SKU { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // ─── NEW navigation to attributes
        public ICollection<ProductVariantAttribute> Attributes { get; set; }
    }
}
