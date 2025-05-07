using AttarStore.Application.Dtos.Catalog;
using AttarStore.Domain.Entities.Catalog;
using AutoMapper;

namespace AttarStore.Application.MappingProfiles
{
    public class CategoryRequestProfile : Profile
    {
        public CategoryRequestProfile()
        {
            // Vendor → CategoryRequest entity
            CreateMap<CategoryRequestCreateDto, CategoryRequest>();

            // Entity → shared DTO
            CreateMap<CategoryRequest, CategoryRequestDto>()
                .ForMember(d => d.VendorName,
                           o => o.MapFrom(src => src.Vendor.Name));

            // Admin → update request
            CreateMap<CategoryRequestUpdateDto, CategoryRequest>()
                .ForMember(d => d.Status,
                           o => o.MapFrom(src => src.Status))
                .ForMember(d => d.ResponseMessage,
                           o => o.MapFrom(src => src.ResponseMessage));
        }
    }
}
