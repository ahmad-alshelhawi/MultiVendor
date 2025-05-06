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
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly AppDbContext _db;
        public ProductVariantRepository(AppDbContext db) => _db = db;

        public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId)
        {
            return await _db.ProductVariants
                .Include(v => v.Images)
                .Include(v => v.Attributes)
                    .ThenInclude(pa => pa.VariantOption)
                .Include(v => v.Attributes)
                    .ThenInclude(pa => pa.VariantOptionValue)
                .Where(v => v.ProductId == productId)
                .ToArrayAsync();
        }

        public async Task<ProductVariant> GetByIdAsync(int variantId)
        {
            return await _db.ProductVariants
                .Include(v => v.Images)
                .Include(v => v.Attributes)
                    .ThenInclude(pa => pa.VariantOption)
                .Include(v => v.Attributes)
                    .ThenInclude(pa => pa.VariantOptionValue)
                .FirstOrDefaultAsync(v => v.Id == variantId);
        }

        public async Task AddAsync(ProductVariant variant)
        {
            _db.ProductVariants.Add(variant);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductVariant variant)
        {
            var existing = await _db.ProductVariants
                .Include(v => v.Attributes)
                .Include(v => v.Images)
                .FirstOrDefaultAsync(v => v.Id == variant.Id);

            if (existing == null) return;

            // sync attributes
            _db.RemoveRange(existing.Attributes
                .Where(a => !variant.Attributes
                    .Any(x => x.VariantOptionValueId == a.VariantOptionValueId &&
                              x.VariantOptionId == a.VariantOptionId)));

            existing.Attributes = variant.Attributes;

            // sync images
            _db.RemoveRange(existing.Images
                .Where(img => !variant.Images
                    .Any(x => x.Id == img.Id)));
            existing.Images = variant.Images;

            // update scalar props
            _db.Entry(existing).CurrentValues.SetValues(variant);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int variantId)
        {
            var entity = await _db.ProductVariants.FindAsync(variantId);
            if (entity != null)
            {
                _db.ProductVariants.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
