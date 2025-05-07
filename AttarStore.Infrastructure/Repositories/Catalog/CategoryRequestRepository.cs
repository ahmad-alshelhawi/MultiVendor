// AttarStore.Infrastructure.Repositories.Catalog/CategoryRequestRepository.cs
using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Repositories.Catalog
{
    public class CategoryRequestRepository : ICategoryRequestRepository
    {
        private readonly AppDbContext _db;
        public CategoryRequestRepository(AppDbContext db)
            => _db = db;

        public async Task<CategoryRequest> AddAsync(CategoryRequest entity)
        {
            _db.CategoryRequests.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }


        public async Task<IEnumerable<CategoryRequest>> GetAllAsync()
        {
            return await _db.CategoryRequests
                            .Include(cr => cr.Vendor)
                            .OrderByDescending(cr => cr.CreatedAt)
                            .ToListAsync();
        }

        public async Task<IEnumerable<CategoryRequest>> GetByVendorAsync(int vendorId)
        {
            return await _db.CategoryRequests
                            .Where(cr => cr.VendorId == vendorId)
                            .OrderByDescending(cr => cr.CreatedAt)
                            .ToListAsync();
        }

        public async Task<CategoryRequest?> GetByIdAsync(int id)
        {
            return await _db.CategoryRequests
                            .Include(cr => cr.Vendor)
                            .FirstOrDefaultAsync(cr => cr.Id == id);
        }

        public async Task UpdateAsync(CategoryRequest request)
        {
            _db.Entry(request).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
    }
}
