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
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<RolePermission, RolePermissionDto>().ReverseMap();
        }
    }

}
