// AttarStore.Domain.Interfaces.Catalog/ICategoryRequestRepository.cs
using AttarStore.Domain.Entities.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces.Catalog
{
    public interface ICategoryRequestRepository
    {
        Task<IEnumerable<CategoryRequest>> GetAllAsync();
        Task<IEnumerable<CategoryRequest>> GetByVendorAsync(int vendorId);
        Task<CategoryRequest?> GetByIdAsync(int id);
 
        Task<CategoryRequest> AddAsync(CategoryRequest entity);

        Task UpdateAsync(CategoryRequest request);
    }
}
