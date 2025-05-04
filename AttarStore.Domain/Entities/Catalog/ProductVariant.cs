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
        public int Stock { get; set; }

        /// <summary>
        /// JSON blob or serialized attributes (e.g. size/color).
        /// </summary>
        public string AttributesJson { get; set; }
    }
}
