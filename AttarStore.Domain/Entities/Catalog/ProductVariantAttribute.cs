using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Entities.Catalog
{
    public class ProductVariantAttribute
    {
        // Composite key: ProductVariantId + VariantOptionId + VariantOptionValueId
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }

        public int VariantOptionId { get; set; }
        public VariantOption VariantOption { get; set; }

        public int VariantOptionValueId { get; set; }
        public VariantOptionValue VariantOptionValue { get; set; }
    }
}
