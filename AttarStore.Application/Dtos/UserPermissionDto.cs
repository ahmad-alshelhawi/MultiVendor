using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.Dtos
{
    public class UserPermissionDto
    {
        public string PermissionName { get; set; }
        public bool IsGranted { get; set; }
    }

}
