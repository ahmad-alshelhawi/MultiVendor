using AttarStore.Domain.Entities;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces
{
    public interface IVendorRepository
    {
        /// <summary>
        /// Checks if a vendor name or contact email is already in use by any Vendor, Admin, User, or Client.
        /// </summary>
        Task<bool> ExistsByNameOrContactEmailAsync(
            string name,
            string contactEmail,
            int? excludeVendorId = null);

        Task<Vendor[]> GetAllAsync();
        Task<Vendor> GetByIdAsync(int id);
        Task<Vendor> GetByNameAsync(string name);
        Task<Vendor> GetByContactEmailAsync(string email);
            
        Task AddAsync(Vendor vendor);
        Task UpdateAsync(Vendor vendor);

        /// <summary>
        /// Soft‐deletes a vendor by setting IsActive = false.
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Persists pending changes.
        /// </summary>
        Task<bool> SaveChangesAsync();
    }
}
