using Microsoft.AspNetCore.Authorization;

namespace AttarStore.WebApi.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; }

        public PermissionRequirement(string permissionName)
            => PermissionName = permissionName;
    }
}
