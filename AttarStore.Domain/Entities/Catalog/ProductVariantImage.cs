using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Entities.Catalog
{
    public class ProductVariantImage
    {
        public int Id { get; set; }

        // URL or path to the stored file
        [Required]
        public string Url { get; set; }

        // link back to variant
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}
