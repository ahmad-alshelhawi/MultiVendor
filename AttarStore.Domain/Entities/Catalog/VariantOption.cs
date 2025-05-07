using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Entities.Catalog
{
    public class VariantOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public ICollection<VariantOptionValue> Values { get; set; }
            = new List<VariantOptionValue>();
    }
}
