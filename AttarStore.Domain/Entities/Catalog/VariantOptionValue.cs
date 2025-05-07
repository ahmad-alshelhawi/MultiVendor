using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Entities.Catalog
{
    public class VariantOptionValue
    {
        public int Id { get; set; }
        public int VariantOptionId { get; set; }
        public VariantOption? VariantOption { get; set; }

        public string Value { get; set; } = "";
    }
}
