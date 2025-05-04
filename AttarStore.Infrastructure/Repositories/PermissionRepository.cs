using System;
using System.Linq;
using System.Threading.Tasks;
using AttarStore.Services.Data;
using AttarStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using AttarStore.Domain.Entities.Auth;

namespace AttarStore.Services.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly AppDbContext _db;
        public PermissionRepository(AppDbContext dbContext)
            => _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<Permission[]> GetAllAsync()
            => await _db.Permissions.AsNoTracking().ToArrayAsync();

        public async Task<Permission> GetByIdAsync(int id)
            => await _db.Permissions.FindAsync(id);

        public async Task<Permission> GetByNameAsync(string name)
            => await _db.Permissions
                 .AsNoTracking()
                 .FirstOrDefaultAsync(p => p.Name == name);

        public async Task<bool> ExistsByNameAsync(string name)
            => await _db.Permissions.AnyAsync(p => p.Name == name);

        public async Task AddAsync(Permission permission)
        {
            if (permission is null)
                throw new ArgumentNullException(nameof(permission));
            if (await ExistsByNameAsync(permission.Name))
                throw new InvalidOperationException("Permission name already exists.");

            _db.Permissions.Add(permission);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Permission permission)
        {
            if (permission is null)
                throw new ArgumentNullException(nameof(permission));
            if (await _db.Permissions
                      .AnyAsync(p => p.Id != permission.Id && p.Name == permission.Name))
                throw new InvalidOperationException("Permission name already exists.");

            _db.Entry(permission).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var permission = await _db.Permissions.FindAsync(id);
            if (permission != null)
            {
                // Remove related RolePermissions first
                var deps = _db.RolePermissions.Where(rp => rp.PermissionId == id);
                _db.RolePermissions.RemoveRange(deps);

                _db.Permissions.Remove(permission);
                await _db.SaveChangesAsync();
            }
        }
    }
}
