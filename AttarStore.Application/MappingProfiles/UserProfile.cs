using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities;

using AutoMapper;

namespace AttarStore.Api.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserMapperView>();

            CreateMap<UserMapperCreate, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<UserMapperUpdate, User>()
                .ForAllMembers(opts => opts.Condition((src, _, srcMember) => srcMember != null));

            CreateMap<UserProfileUpdateMapper, User>()
                .ForAllMembers(opts => opts.Condition((src, _, srcMember) => srcMember != null));
        }
    }
}
