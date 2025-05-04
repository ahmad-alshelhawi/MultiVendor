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
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            // ─── Order → View
            CreateMap<Order, OrderView>()
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.Items));

            CreateMap<OrderItem, OrderItemView>();

            // ─── Create DTO → Order
            CreateMap<OrderCreate, Order>()
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src =>
                               src.Items.ConvertAll(i => new OrderItem
                               {
                                   ProductVariantId = i.ProductVariantId,
                                   Quantity = i.Quantity
                               })));

            // ─── Status‐Update DTO → Order
            CreateMap<OrderStatusUpdate, Order>()
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status));
        }
    }

}
