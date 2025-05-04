using System;
using System.Linq;
using System.Threading.Tasks;
using AttarStore.Domain.Entities.Catalog;
using AttarStore.Services.Data;
using AttarStore.Services.Interfaces.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AttarStore.Services.Repositories.Catalog
{
    public class SubcategoryRepository : ISubcategoryRepository
    {
        private readonly AppDbContext _db;
        public SubcategoryRepository(AppDbContext db) => _db = db;

        public async Task<Subcategory[]> GetByCategoryIdAsync(int categoryId)
            => await _db.Subcategories
                .Where(sc => sc.CategoryId == categoryId)
                .AsNoTracking()
                .ToArrayAsync();

        public async Task<Subcategory> GetByIdAsync(int id)
            => await _db.Subcategories
                .AsNoTracking()
                .FirstOrDefaultAsync(sc => sc.Id == id);

        public async Task AddAsync(Subcategory sub)
        {
            if (sub == null) throw new ArgumentNullException(nameof(sub));
            if (!await _db.Categories.AnyAsync(c => c.Id == sub.CategoryId))
                throw new InvalidOperationException("Parent category not found.");
            if (await _db.Subcategories
                  .AnyAsync(s => s.CategoryId == sub.CategoryId && s.Name == sub.Name))
                throw new InvalidOperationException("Subcategory name already exists in this category.");

            _db.Subcategories.Add(sub);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Subcategory sub)
        {
            if (sub == null) throw new ArgumentNullException(nameof(sub));
            if (await _db.Subcategories
                  .AnyAsync(s => s.Id != sub.Id
                               && s.CategoryId == sub.CategoryId
                               && s.Name == sub.Name))
                throw new InvalidOperationException("Subcategory name already exists in this category.");

            _db.Entry(sub).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var sub = await _db.Subcategories.FindAsync(id);
            if (sub == null) return;
            _db.Subcategories.Remove(sub);
            await _db.SaveChangesAsync();
        }
    }
}
