using Microsoft.AspNetCore.Http;
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
        public string? Subcategory { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<ProductVariantMapperView> Variants { get; set; }
    }

    // ─── Product Create ──────────────────────────────────────────────────────
    public class ProductMapperCreate
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
        public string Details { get; set; }
        public int? SubcategoryId { get; set; }
        [Required] public int VendorId { get; set; }

        // ← Add this:
        /// <summary>
        /// One or more variants for this product (price, SKU, stock, attributes…)
        /// </summary>
        public List<ProductVariantCreateDto>? Variants { get; set; }
            = new List<ProductVariantCreateDto>();
    }

    // ─── Product Update ──────────────────────────────────────────────────────
    public class ProductMapperUpdate
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Details { get; set; }
        public int? SubcategoryId { get; set; }
    }
    // ─── Variant View ────────────────────────────────────────────────────────
    public class ProductVariantMapperView
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public List<VariantAttributeViewDto> Attributes { get; set; }
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
        public string Value { get; set; }
    }
    public class ProductVariantCreateDto
    {
        [Required]
        public string SKU { get; set; }

        [Required]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        /// <summary>
        /// One or more option/value pairs, e.g. Size=Large, Color=Blue
        /// </summary>
        public List<VariantAttributeDto>? Attributes { get; set; }
            = new List<VariantAttributeDto>();
    }

    public class ProductImageUploadDto
    {
        [Required] public IFormFile File { get; set; }
    }
    public class VariantOptionCreateDto
    {
        [Required] public string Name { get; set; }
    }

    public class VariantOptionValueCreateDto
    {
        [Required] public int OptionId { get; set; }
        [Required] public string Value { get; set; }
    }

}
