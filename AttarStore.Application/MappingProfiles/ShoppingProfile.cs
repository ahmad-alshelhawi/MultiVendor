using AttarStore.Application.Dtos.Shopping;
using AttarStore.Domain.Entities.Shopping;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.MappingProfiles
{
    public class ShoppingProfile : Profile
    {
        public ShoppingProfile()
        {
            CreateMap<InventoryTransactionCreateDto, InventoryTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())   // set in controller
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp ?? DateTime.UtcNow));

            CreateMap<InventoryTransaction, InventoryTransactionDto>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(src => src.User.Name));
        }
    }
}
