// AttarStore.Application/MappingProfiles/NotificationProfile.cs
using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities.Auth; // adjust if your Notification entity lives elsewhere
using AutoMapper;

namespace AttarStore.Application.MappingProfiles
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<Notification, NotificationDto>();
            CreateMap<CreateNotificationDto, Notification>();
        }
    }
}