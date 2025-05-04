using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities;
using AutoMapper;

namespace AttarStore.Application.MappingProfiles
{
    public class VendorProfile : Profile
    {
        public VendorProfile()
        {
            CreateMap<Vendor, VendorMapperView>();

            CreateMap<VendorMapperCreate, Vendor>();

            CreateMap<VendorMapperUpdate, Vendor>()
                .ForAllMembers(opts => opts.Condition((src, _, srcMember) => srcMember != null));

            CreateMap<VendorProfileUpdateMapper, Vendor>()
                .ForAllMembers(opts => opts.Condition((src, _, srcMember) => srcMember != null));
        }
    }
}
