using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.Dtos
{
    // PermissionDto.cs
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    // RolePermissionDto.cs
    public class RolePermissionDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public int PermissionId { get; set; }
    }

}
