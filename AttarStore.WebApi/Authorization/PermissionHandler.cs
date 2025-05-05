// WebApi/Authorization/PermissionHandler.cs
using AttarStore.Domain.Entities.Auth;
using AttarStore.Services.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AttarStore.WebApi.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly AppDbContext _db;

        public PermissionHandler(AppDbContext db)
        {
            _db = db;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // 1) Get user ID & role from claims
            var idClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (!int.TryParse(idClaim, out var userId) || string.IsNullOrEmpty(role))
            {
                context.Fail();
                return;
            }

            // 2) Check user‐level overrides first
            var userPerm = await _db.UserPermissions
                .AsNoTracking()
                .SingleOrDefaultAsync(up =>
                    up.UserId == userId
                    && up.PermissionName == requirement.Permission);

            if (userPerm != null)
            {
                if (userPerm.IsGranted)
                    context.Succeed(requirement);
                else
                    context.Fail();
                return;
            }

            // 3) Fall back to role‐based permissions
            bool hasRolePerm = await _db.RolePermissions
                .AsNoTracking()
                .Include(rp => rp.Permission)
                .AnyAsync(rp =>
                    rp.RoleName == role
                    && rp.Permission.Name == requirement.Permission);

            if (hasRolePerm)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
