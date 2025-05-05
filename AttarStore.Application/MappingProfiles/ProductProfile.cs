using AutoMapper;
using AttarStore.Application.Dtos.Catalog;
using AttarStore.Domain.Entities.Catalog;

namespace AttarStore.Application.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // ─── Product Mappings ────────────────────────────────────────

            // Create DTO → Entity
            CreateMap<ProductMapperCreate, Product>();

            // Update DTO → Entity
            CreateMap<ProductMapperUpdate, Product>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Entity → View DTO
            CreateMap<Product, ProductMapperView>()
                .ForMember(vm => vm.ImageUrls,
                           opt => opt.MapFrom(p => p.Images.Select(img => img.Url)))
                .ForMember(vm => vm.Variants,
                           opt => opt.MapFrom(p => p.Variants));
            CreateMap<ProductMapperCreate, Product>()
            .ForMember(dest => dest.Images,
                       opt => opt.Ignore())
            .ForMember(dest => dest.Variants,
                       opt => opt.MapFrom(src => src.Variants));

            // ─── Variant‐Option Metadata ─────────────────────────────────

            CreateMap<VariantOption, VariantOptionDto>();
            CreateMap<VariantOptionValue, VariantOptionValueDto>();

            // ─── ProductVariant Mappings ────────────────────────────────

            // Create DTO → Entity
            CreateMap<ProductVariantCreateDto, ProductVariant>();

            // Entity → View DTO
            CreateMap<ProductVariant, ProductVariantMapperView>()
                .ForMember(vm => vm.Attributes,
                           opt => opt.MapFrom(v => v.Attributes));

            // ─── Variant‐Attribute Mapping ──────────────────────────────

            CreateMap<ProductVariantAttribute, VariantAttributeViewDto>()
                .ForMember(vm => vm.OptionId, opt => opt.MapFrom(pa => pa.VariantOptionId))
                .ForMember(vm => vm.ValueId, opt => opt.MapFrom(pa => pa.VariantOptionValueId))
                .ForMember(vm => vm.OptionName, opt => opt.MapFrom(pa => pa.VariantOption.Name))
                .ForMember(vm => vm.Value, opt => opt.MapFrom(pa => pa.VariantOptionValue.Value));

            // CreateDto → Attribute join
            CreateMap<VariantAttributeDto, ProductVariantAttribute>();


            CreateMap<ProductVariantAttribute, VariantAttributeViewDto>()
            .ForMember(vm => vm.OptionId, opt => opt.MapFrom(pa => pa.VariantOptionId))
            .ForMember(vm => vm.ValueId, opt => opt.MapFrom(pa => pa.VariantOptionValueId))
            .ForMember(vm => vm.OptionName, opt => opt.MapFrom(pa => pa.VariantOption.Name))
            .ForMember(vm => vm.Value, opt => opt.MapFrom(pa => pa.VariantOptionValue.Value));

            CreateMap<VariantAttributeDto, ProductVariantAttribute>();


            CreateMap<VariantOption, VariantOptionDto>();
            CreateMap<VariantOptionCreateDto, VariantOption>();

            CreateMap<VariantOptionValue, VariantOptionValueDto>();
            CreateMap<VariantOptionValueCreateDto, VariantOptionValue>();
        }
    }
}
