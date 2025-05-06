// AttarStore.Application.Dtos.Catalog/ProductDtos.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttarStore.Application.Dtos.Catalog
{
    // ─── Create DTO ────────────────────────────────────────────────────────────
    public class ProductMapperCreate
    {
        [Required] public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public int? SubcategoryId { get; set; }

        // if you want the product itself (no variants) to carry stock & price:
        public decimal? DefaultPrice { get; set; }
        public int? DefaultStock { get; set; }

        public List<ProductVariantCreateDto> Variants { get; set; }
    }

    // ─── Update DTO ────────────────────────────────────────────────────────────
    public class ProductMapperUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public int? SubcategoryId { get; set; }
        public decimal? DefaultPrice { get; set; }
        public int? DefaultStock { get; set; }

        public List<ProductVariantUpdateDto> Variants { get; set; }
    }

    // ─── View DTO ──────────────────────────────────────────────────────────────
    public class ProductMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public int? SubcategoryId { get; set; }
        public string Subcategory { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }

        public decimal? DefaultPrice { get; set; }
        public int? DefaultStock { get; set; }

        public List<ProductVariantMapperView> Variants { get; set; }
    }

    // ─── Variant “create” DTO ─────────────────────────────────────────────────
    public class ProductVariantCreateDto
    {
        [Required] public string Sku { get; set; }
        [Required] public decimal Price { get; set; }
        [Required] public int Stock { get; set; }
        public List<VariantAttributeCreateDto> Attributes { get; set; }
    }

    // ─── Variant “update” DTO ─────────────────────────────────────────────────
    public class ProductVariantUpdateDto : ProductVariantCreateDto
    {
        [Required] public int Id { get; set; }
    }

    // ─── Variant view DTO ─────────────────────────────────────────────────────
    public class ProductVariantMapperView
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public List<VariantAttributeViewDto> Attributes { get; set; }
    }

    // ─── Attribute create DTO ─────────────────────────────────────────────────
    public class VariantAttributeCreateDto
    {
        [Required] public int OptionId { get; set; }
        [Required] public int ValueId { get; set; }
    }

    // ─── Attribute view DTO ──────────────────────────────────────────────────
    public class VariantAttributeViewDto
    {
        public int OptionId { get; set; }
        public string OptionName { get; set; }
        public int ValueId { get; set; }
        public string ValueName { get; set; }
    }

    // ─── Option & Value metadata DTOs ─────────────────────────────────────────
    public class VariantOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class VariantOptionValueDto
    {
        public int Id { get; set; }
        public int OptionId { get; set; }
        public string Value { get; set; }
    }
}
