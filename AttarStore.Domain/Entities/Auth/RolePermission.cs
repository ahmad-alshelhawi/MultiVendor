namespace AttarStore.Domain.Entities.Auth
{
    public class RolePermission
    {
        public int Id { get; set; }
        public string RoleName { get; set; }   // must match one of Roles.Admin, etc.
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}
