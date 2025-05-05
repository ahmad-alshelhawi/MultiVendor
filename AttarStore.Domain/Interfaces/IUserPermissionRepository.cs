using AttarStore.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces
{
    public interface IUserPermissionRepository
    {
        Task<IEnumerable<UserPermission>> GetByUserIdAsync(int userId);
        Task<UserPermission> UpsertAsync(int userId, string permissionName, bool isGranted);
        Task DeleteAsync(int userId, string permissionName);
    }
}
