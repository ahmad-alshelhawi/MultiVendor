using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Repositories
{
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly AppDbContext _db;

        public UserPermissionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<UserPermission>> GetByUserIdAsync(int userId)
        {
            return await _db.UserPermissions
                .AsNoTracking()
                .Where(up => up.UserId == userId)
                .ToListAsync();
        }

        public async Task<UserPermission> UpsertAsync(int userId, string permissionName, bool isGranted)
        {
            var up = await _db.UserPermissions
                .SingleOrDefaultAsync(x =>
                    x.UserId == userId
                    && x.PermissionName == permissionName);

            if (up == null)
            {
                up = new UserPermission
                {
                    UserId = userId,
                    PermissionName = permissionName,
                    IsGranted = isGranted
                };
                _db.UserPermissions.Add(up);
            }
            else
            {
                up.IsGranted = isGranted;
                _db.UserPermissions.Update(up);
            }

            await _db.SaveChangesAsync();
            return up;
        }

        public async Task DeleteAsync(int userId, string permissionName)
        {
            var up = await _db.UserPermissions
                .SingleOrDefaultAsync(x =>
                    x.UserId == userId
                    && x.PermissionName == permissionName);

            if (up != null)
            {
                _db.UserPermissions.Remove(up);
                await _db.SaveChangesAsync();
            }
        }
    }
}
