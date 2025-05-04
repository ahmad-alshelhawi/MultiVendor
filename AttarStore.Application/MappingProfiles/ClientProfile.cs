using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities;
using AutoMapper;

namespace AttarStore.Api.Profiles
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<Client, ClientMapperView>();

            CreateMap<ClientMapperCreate, Client>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<ClientMapperUpdate, Client>()
                .ForAllMembers(opts => opts.Condition((src, _, srcMember) => srcMember != null));

            CreateMap<ClientProfileUpdateMapper, Client>()
                .ForAllMembers(opts => opts.Condition((src, _, srcMember) => srcMember != null));
        }
    }
}
