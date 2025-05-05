// WebApi/Authorization/PermissionRequirement.cs
using Microsoft.AspNetCore.Authorization;

namespace AttarStore.WebApi.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
    }
}
