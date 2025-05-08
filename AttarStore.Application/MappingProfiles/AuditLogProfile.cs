using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities.Auth;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.MappingProfiles
{
    public class AuditLogProfile : Profile
    {
        public AuditLogProfile()
        {
            CreateMap<AuditLog, AuditLogDto>()
                .ForMember(d => d.ActorId, o => o.MapFrom(s => s.ActorId))
                .ForMember(d => d.ActorType, o => o.MapFrom(s => s.ActorType))
                .ForMember(d => d.ActorName, o => o.MapFrom(s => s.ActorName))
                .ForMember(d => d.ActorRole, o => o.MapFrom(s => s.ActorRole))
                .ForMember(d => d.Action, o => o.MapFrom(s => s.Action))
                .ForMember(d => d.EntityType, o => o.MapFrom(s => s.EntityType))
                .ForMember(d => d.EntityId, o => o.MapFrom(s => s.EntityId))
                .ForMember(d => d.EntityName, o => o.MapFrom(s => s.EntityName))
                .ForMember(d => d.Timestamp, o => o.MapFrom(s => s.Timestamp))
                .ForMember(d => d.Details, o => o.MapFrom(s => s.Details));
        }
    }
}
