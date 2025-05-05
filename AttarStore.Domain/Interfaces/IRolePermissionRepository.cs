using AttarStore.Domain.Entities.Auth;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces
{
    public interface IRolePermissionRepository
    {
        Task<RolePermission[]> GetAllAsync();
        Task<RolePermission[]> GetByRoleAsync(string roleName);
        Task<RolePermission> GetByIdAsync(int id);


        Task<RolePermission> AddAsync(RolePermission entity);

        Task DeleteAsync(int id);
    }
}
