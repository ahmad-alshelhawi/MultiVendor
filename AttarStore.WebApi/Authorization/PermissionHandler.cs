using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AttarStore.Services.Data;      // your DbContext namespace
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AttarStore.WebApi.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly AppDbContext _db;

        public PermissionHandler(AppDbContext db)
            => _db = db;

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // 1. Read the user's role claim
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(role))
                return;

            // 2. Check RolePermissions table for this role + permission
            var hasPermission = await _db.RolePermissions
                .AsNoTracking()
                .Include(rp => rp.Permission)
                .AnyAsync(rp =>
                    rp.RoleName == role &&
                    rp.Permission.Name == requirement.PermissionName);

            if (hasPermission)
                context.Succeed(requirement);
        }
    }
}
