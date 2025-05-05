using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Repositories.Catalog
{
    public class CategoryRequestRepository : ICategoryRequestRepository
    {
        private readonly AppDbContext _db;
        public CategoryRequestRepository(AppDbContext db) => _db = db;

        public Task<CategoryRequest[]> GetByVendorAsync(int vId) =>
          _db.CategoryRequests.Where(r => r.VendorId == vId).ToArrayAsync();

        public Task<CategoryRequest[]> GetAllPendingAsync() =>
          _db.CategoryRequests.Where(r => r.Status == RequestStatus.Pending).ToArrayAsync();

        public async Task<CategoryRequest> CreateAsync(CategoryRequest req)
        {
            _db.CategoryRequests.Add(req);
            await _db.SaveChangesAsync();
            return req;
        }

        public async Task<CategoryRequest> UpdateStatusAsync(int id, RequestStatus status)
        {
            var req = await _db.CategoryRequests.FindAsync(id);
            req.Status = status;
            await _db.SaveChangesAsync();
            return req;
        }
    }

}
