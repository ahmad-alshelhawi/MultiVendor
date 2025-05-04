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
                    opt => opt.MapFrom(src => src.Variants));

            // ─── CreateDTO → Product ───────────────────────────────────────
            CreateMap<ProductMapperCreate, Product>()
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src =>
                        src.ImageUrls.Select(url => new ProductImage { Url = url })))
                .ForMember(dest => dest.Variants,
                    opt => opt.MapFrom(src => src.Variants));

            // ─── UpdateDTO → Product (skip nulls) ───────────────────────────
            CreateMap<ProductMapperUpdate, Product>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));

            // ─── Variant mappings ────────────────────────────────────────────
            CreateMap<ProductVariant, ProductVariantMapperView>();
            CreateMap<ProductVariantCreateMapper, ProductVariant>();
        }
    }
}
