using System;
using System.Linq;
using System.Threading.Tasks;
using AttarStore.Services.Data;
using AttarStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using AttarStore.Domain.Entities.Auth;

namespace AttarStore.Services.Repositories
{
    public class RolePermissionRepository : IRolePermissionRepository
    {
        private readonly AppDbContext _db;
        public RolePermissionRepository(AppDbContext dbContext)
            => _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<RolePermission[]> GetAllAsync()
            => await _db.RolePermissions
                .Include(rp => rp.Permission)
                .AsNoTracking()
                .ToArrayAsync();

        public async Task<RolePermission[]> GetByRoleAsync(string roleName)
            => await _db.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleName == roleName)
                .AsNoTracking()
                .ToArrayAsync();

        public async Task<RolePermission> GetByIdAsync(int id)
            => await _db.RolePermissions.FindAsync(id);

        public async Task AddAsync(string roleName, int permissionId)
        {
            if (!await _db.Permissions.AnyAsync(p => p.Id == permissionId))
                throw new InvalidOperationException("Permission not found.");

            if (await _db.RolePermissions
                      .AnyAsync(rp => rp.RoleName == roleName && rp.PermissionId == permissionId))
                throw new InvalidOperationException("This role already has that permission.");

            var rp = new RolePermission { RoleName = roleName, PermissionId = permissionId };
            _db.RolePermissions.Add(rp);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var rp = await _db.RolePermissions.FindAsync(id);
            if (rp != null)
            {
                _db.RolePermissions.Remove(rp);
                await _db.SaveChangesAsync();
            }
        }
    }
}
