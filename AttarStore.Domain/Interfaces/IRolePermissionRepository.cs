using AttarStore.Domain.Entities.Auth;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces
{
    public interface IRolePermissionRepository
    {
        Task<RolePermission[]> GetAllAsync();
        Task<RolePermission[]> GetByRoleAsync(string roleName);
        Task<RolePermission> GetByIdAsync(int id);

        Task AddAsync(string roleName, int permissionId);
        Task DeleteAsync(int id);
    }
}
