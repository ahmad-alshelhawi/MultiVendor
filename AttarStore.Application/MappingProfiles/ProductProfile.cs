using AttarStore.Application.Dtos.Catalog;
using AttarStore.Domain.Entities.Catalog;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            // ─── Product → ViewDTO ────────────────────────────────────────
            CreateMap<Product, ProductMapperView>()
                .ForMember(dest => dest.ImageUrls,
                    opt => opt.MapFrom(src => src.Images.Select(i => i.Url)))
                .ForMember(dest => dest.Variants,
                    opt => opt.MapFrom(src => src.Variants))
                 .ForMember(d => d.VendorName, opt => opt.MapFrom(src => src.Vendor.Name));

            // ─── CreateDTO → Product ───────────────────────────────────────
            CreateMap<ProductMapperCreate, Product>()
            .ForMember(dest => dest.Variants,
                       opt => opt.MapFrom(src => src.Variants));

            // ─── UpdateDTO → Product (skip nulls) ───────────────────────────
            CreateMap<ProductMapperUpdate, Product>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));

            // ─── Variant mappings ────────────────────────────────────────────
            CreateMap<ProductVariant, ProductVariantMapperView>();
            CreateMap<ProductVariantCreateMapper, ProductVariant>();


            // Option & Value
            CreateMap<VariantOption, VariantOptionDto>();
            CreateMap<VariantOptionValue, VariantOptionValueDto>();

            // ProductVariant → View
            CreateMap<ProductVariant, ProductVariantMapperView>()
                .ForMember(dest => dest.SelectedOptions,
                           opt => opt.MapFrom(src => src.Attributes.Select(a => a.VariantOptionValue)));

            // Create DTO → ProductVariant + join‐table
            CreateMap<ProductVariantCreateDto, ProductVariant>()
                .ForMember(dest => dest.Attributes,
                    opt => opt.MapFrom(src =>
                        src.OptionValueIds.Select(valId => new ProductVariantAttribute
                        {
                            VariantOptionValueId = valId
                        })));




        }
    }
}
