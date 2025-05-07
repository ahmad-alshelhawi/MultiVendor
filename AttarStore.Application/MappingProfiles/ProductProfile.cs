using AttarStore.Application.Dtos.Catalog;
using AttarStore.Application.Dtos.Shopping;
using AttarStore.Domain.Entities.Catalog;
using AutoMapper;
using System.Linq;

namespace AttarStore.Application.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // ─── Product → View DTO ───────────────────────────────────────
            CreateMap<Product, ProductMapperView>()
                .ForMember(d => d.DefaultPrice, o => o.MapFrom(src => src.DefaultPrice))
                .ForMember(d => d.DefaultStock, o => o.MapFrom(src => src.DefaultStock))
                .ForMember(d => d.Subcategory, o => o.MapFrom(src => src.Subcategory != null ? src.Subcategory.Name : null))
                .ForMember(d => d.VendorName, o => o.MapFrom(src => src.Vendor != null ? src.Vendor.Name : null))
                .ForMember(d => d.Variants, o => o.MapFrom(src => src.Variants))
            .ForMember(d => d.ImageUrls, o => o.MapFrom(src => src.Images.Select(i => i.Url)));


            // ─── Create DTO → Product ────────────────────────────────────
            CreateMap<ProductMapperCreate, Product>()
                .ForMember(d => d.Images, o => o.Ignore()) // handled separately if you want image‐upload endpoint
                .ForMember(d => d.DefaultPrice, o => o.MapFrom(src => src.DefaultPrice ?? 0))
                .ForMember(d => d.DefaultStock, o => o.MapFrom(src => src.DefaultStock ?? 0))
                .ForMember(d => d.Variants, o => o.MapFrom(src => src.Variants));

            // ─── Update DTO → Product ────────────────────────────────────
            CreateMap<ProductMapperUpdate, Product>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // ─── Variant → View DTO ──────────────────────────────────────
            CreateMap<ProductVariant, ProductVariantMapperView>()
                .ForMember(d => d.Attributes, o => o.MapFrom(src => src.Attributes))
                .ForMember(d => d.ImageUrls, o => o.MapFrom(src => src.Images.Select(i => i.Url)));


            // ─── Variant Create DTO → Variant ────────────────────────────
            CreateMap<ProductVariantCreateDto, ProductVariant>()
                    // null or zero → we’ll back-fill defaults in repo
                    .ForMember(d => d.Price, o => o.MapFrom(src => src.Price ?? 0M))
                    .ForMember(d => d.Stock, o => o.MapFrom(src => src.Stock ?? 0))
                    .ForMember(d => d.Images,
                    o => o.MapFrom(src => src.ImageUrls.Select(u => new ProductVariantImage { Url = u })))
                                .ForMember(d => d.Attributes,
                    o => o.MapFrom(src => src.Attributes));

            // ─── Variant Update DTO → Variant ────────────────────────────
            CreateMap<ProductVariantUpdateDto, ProductVariant>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            // ─── Attribute entity → View DTO ─────────────────────────────
            CreateMap<ProductVariantAttribute, VariantAttributeViewDto>()
                .ForMember(d => d.OptionId, o => o.MapFrom(src => src.VariantOptionId))
                .ForMember(d => d.ValueId, o => o.MapFrom(src => src.VariantOptionValueId))
                .ForMember(d => d.OptionName, o => o.MapFrom(src => src.VariantOption.Name))
                .ForMember(d => d.ValueName, o => o.MapFrom(src => src.VariantOptionValue.Value));

            // ─── Attribute Create DTO → Attribute entity ─────────────────
            CreateMap<VariantAttributeCreateDto, ProductVariantAttribute>()
                 // map only the two FK properties
                 .ForMember(dest => dest.VariantOptionId,
                            opt => opt.MapFrom(src => src.OptionId))
                 .ForMember(dest => dest.VariantOptionValueId,
                            opt => opt.MapFrom(src => src.ValueId))
                 // the `ProductVariantId` will be set by the parent entity, so ignore it here:
                 .ForMember(dest => dest.ProductVariantId,
                            opt => opt.Ignore())
                 // ignore any other properties (if you have others)
                 .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
                     // only map when we've explicitly set up a mapping above
                     srcMember != null && (
                       opt.DestinationMember.Name == nameof(ProductVariantAttribute.VariantOptionId) ||
                       opt.DestinationMember.Name == nameof(ProductVariantAttribute.VariantOptionValueId)
                     )
     ));

            // ─── Option & Value metadata ─────────────────────────────────
            CreateMap<VariantOption, VariantOptionDto>();
            CreateMap<VariantOptionValue, VariantOptionValueDto>();

            // ─── InventoryTransaction → DTO ───────────────────────────────
            CreateMap<Domain.Entities.Shopping.InventoryTransaction, InventoryTransactionDto>();
        }
    }
}
