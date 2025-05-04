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
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Cart, CartView>()
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.Items));

            CreateMap<CartItem, CartItemView>();

            CreateMap<CartItemCreate, CartItem>();
            CreateMap<CartItemUpdate, CartItem>()
                .ForMember(dest => dest.Quantity,
                           opt => opt.MapFrom(src => src.Quantity));
        }
    }
}
