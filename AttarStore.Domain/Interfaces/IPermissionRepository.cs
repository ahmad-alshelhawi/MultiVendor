using AttarStore.Domain.Entities.Auth;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces
{
    public interface IPermissionRepository
    {
        Task<Permission[]> GetAllAsync();
        Task<Permission> GetByIdAsync(int id);
        Task<Permission> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);

        Task AddAsync(Permission permission);
        Task UpdateAsync(Permission permission);
        Task DeleteAsync(int id);
    }
}
