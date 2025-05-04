using System;
using System.Linq;
using System.Threading.Tasks;
using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AttarStore.Services.Data;
using AttarStore.Services.Interfaces.Catalog;
using Microsoft.EntityFrameworkCore;

namespace AttarStore.Services.Repositories.Catalog
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _db;
        public CategoryRepository(AppDbContext db) => _db = db;

        public async Task<Category[]> GetAllAsync()
            => await _db.Categories
                .Include(c => c.Subcategories)
                .AsNoTracking()
                .ToArrayAsync();

        public async Task<Category> GetByIdAsync(int id)
            => await _db.Categories
                .Include(c => c.Subcategories)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task AddAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            if (await _db.Categories.AnyAsync(c => c.Name == category.Name))
                throw new InvalidOperationException("Category name already exists.");

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            if (await _db.Categories
                  .AnyAsync(c => c.Id != category.Id && c.Name == category.Name))
                throw new InvalidOperationException("Category name already exists.");

            _db.Entry(category).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var c = await _db.Categories.FindAsync(id);
            if (c == null) return;
            _db.Categories.Remove(c);
            await _db.SaveChangesAsync();
        }
    }
}
