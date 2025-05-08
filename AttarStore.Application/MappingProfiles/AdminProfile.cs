using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities;

using AutoMapper;

namespace AttarStore.Api.Profiles
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            // Entity → View DTO
            CreateMap<Admin, AdminMapperView>();

            // Create DTO → Entity
            CreateMap<AdminMapperCreate, Admin>();


            // Update DTO → Entity
            CreateMap<AdminMapperUpdate, Admin>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Profile‐only DTO → Entity (same as update)
            CreateMap<AdminProfileUpdateMapper, Admin>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
