using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Entities.Auth
{
    public class UserPermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PermissionName { get; set; }  // e.g. "Product.Create"
        public bool IsGranted { get; set; }    // true = explicitly granted, false = explicitly revoked

        public User User { get; set; }
    }

}
