using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.Dtos.Catalog
{
    // ─── Product View ────────────────────────────────────────────────────────
    public class ProductMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public int? SubcategoryId { get; set; }

        public List<string> ImageUrls { get; set; }
        public List<ProductVariantMapperView> Variants { get; set; }
    }

    // ─── Product Create ──────────────────────────────────────────────────────
    public class ProductMapperCreate
    {
        [Required] public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public int? SubcategoryId { get; set; }

        public List<string> ImageUrls { get; set; }
        public List<ProductVariantCreateMapper> Variants { get; set; }
    }

    // ─── Product Update ──────────────────────────────────────────────────────
    public class ProductMapperUpdate
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Details { get; set; }
        public int? SubcategoryId { get; set; }

        public List<string>? ImageUrls { get; set; }
        public List<ProductVariantCreateMapper>? Variants { get; set; }
    }

    // ─── Variant View ────────────────────────────────────────────────────────
    public class ProductVariantMapperView
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        /*        public string AttributesJson { get; set; }
        */
        public IList<VariantOptionValueDto> SelectedOptions { get; set; }

    }

    // ─── Variant Create ──────────────────────────────────────────────────────
    public class ProductVariantCreateMapper
    {
        [Required] public string SKU { get; set; }
        [Required] public decimal Price { get; set; }
        [Required] public int Stock { get; set; }
        public string AttributesJson { get; set; }
    }

    public class VariantOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class VariantOptionValueDto
    {
        public int Id { get; set; }
        public int OptionId { get; set; }
        public string Value { get; set; }
    }
    public class ProductVariantCreateDto
    {
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public IList<int> OptionValueIds { get; set; }
    }


}
