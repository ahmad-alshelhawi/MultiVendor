using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities;

using AutoMapper;

namespace AttarStore.Api.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // Create DTO → Entity
            CreateMap<UserMapperCreate, User>()
                // only overwrite Role if the DTO provided one
                .ForMember(dest => dest.Role,
                           opt => opt.Condition(src => src.Role is not null));

            // Entity → View DTO
            CreateMap<User, UserMapperView>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(u => u.Id))
                .ForMember(dto => dto.Name, opt => opt.MapFrom(u => u.Name))
                .ForMember(dto => dto.Email, opt => opt.MapFrom(u => u.Email))
                .ForMember(dto => dto.Phone, opt => opt.MapFrom(u => u.Phone))
                .ForMember(dto => dto.Address, opt => opt.MapFrom(u => u.Address))
                .ForMember(dto => dto.Role, opt => opt.MapFrom(u => u.Role));


            CreateMap<User, AdminUserMapperView>()
              .ForMember(dto => dto.Id, opt => opt.MapFrom(u => u.Id))
              .ForMember(dto => dto.Name, opt => opt.MapFrom(u => u.Name))
              .ForMember(dto => dto.Email, opt => opt.MapFrom(u => u.Email))
              .ForMember(dto => dto.Phone, opt => opt.MapFrom(u => u.Phone))
              .ForMember(dto => dto.Address, opt => opt.MapFrom(u => u.Address))
              .ForMember(dto => dto.Role, opt => opt.MapFrom(u => u.Role));

        }
    }
}
