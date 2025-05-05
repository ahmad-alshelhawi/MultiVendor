using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<RolePermission> AddAsync(RolePermission entity)
        {
            _db.RolePermissions.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
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
