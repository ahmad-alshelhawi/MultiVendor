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
                // we set Role explicitly in controller, so ignore mapping here:
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            // Entity → View DTO (for returns)
            CreateMap<User, UserMapperView>();

            // Update DTO → Entity
            CreateMap<UserMapperUpdate, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<VendorUserCreate, User>()
    .ForMember(dest => dest.Role, opt => opt.Ignore())
    .ForMember(dest => dest.VendorId, opt => opt.Ignore());
        }
    }
}
