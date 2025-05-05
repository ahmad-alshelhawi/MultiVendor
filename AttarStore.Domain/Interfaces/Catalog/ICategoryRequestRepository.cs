using AttarStore.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces.Catalog
{
    public interface ICategoryRequestRepository
    {
        Task<CategoryRequest[]> GetByVendorAsync(int vendorId);
        Task<CategoryRequest[]> GetAllPendingAsync();
        Task<CategoryRequest> CreateAsync(CategoryRequest req);
        Task<CategoryRequest> UpdateStatusAsync(int requestId, RequestStatus newStatus);
    }
}
