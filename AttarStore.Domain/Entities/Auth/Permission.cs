using System.Collections.Generic;

namespace AttarStore.Domain.Entities.Auth
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }   // e.g. "Product.Create"
        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }
            = new List<RolePermission>();
    }
}
