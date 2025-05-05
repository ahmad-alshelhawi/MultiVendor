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
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;
        public ProductRepository(AppDbContext db) => _db = db;

        public async Task<Product[]> GetAllAsync()
            => await _db.Products
                 .Include(p => p.Vendor)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .AsNoTracking()
                .ToArrayAsync();

        public async Task<Product> GetByIdAsync(int id)
            => await _db.Products
                .Include(p => p.Vendor)
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (await _db.Products.AnyAsync(p => p.Name == product.Name))
                throw new InvalidOperationException("Product name already exists.");

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            if (await _db.Products
                  .AnyAsync(p => p.Id != product.Id && p.Name == product.Name))
                throw new InvalidOperationException("Product name already exists.");

            // Replace Images
            var existingImgs = _db.ProductImages.Where(pi => pi.ProductId == product.Id);
            _db.ProductImages.RemoveRange(existingImgs);
            // Replace Variants
            var existingVars = _db.ProductVariants.Where(v => v.ProductId == product.Id);
            _db.ProductVariants.RemoveRange(existingVars);

            _db.Entry(product).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return;
            _db.Products.Remove(p);
            await _db.SaveChangesAsync();
        }
    }
}
