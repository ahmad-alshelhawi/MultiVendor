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

        public ProductVariantRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId)
        {
            return await _db.ProductVariants
                            .Include(v => v.Attributes)
                                .ThenInclude(pa => pa.VariantOptionValue)
                            .Where(v => v.ProductId == productId)
                            .ToListAsync();
        }

        public async Task<ProductVariant> GetByIdAsync(int variantId)
        {
            return await _db.ProductVariants
                            .Include(v => v.Attributes)
                                .ThenInclude(pa => pa.VariantOptionValue)
                            .FirstOrDefaultAsync(v => v.Id == variantId);
        }

        public async Task<ProductVariant> AddAsync(ProductVariant variant)
        {
            _db.ProductVariants.Add(variant);
            await _db.SaveChangesAsync();
            return variant;
        }

        public async Task UpdateAsync(ProductVariant variant)
        {
            // Ensure EF tracks existing Attributes: remove ones no longer present
            var existing = await _db.ProductVariants
                                    .Include(v => v.Attributes)
                                    .FirstOrDefaultAsync(v => v.Id == variant.Id);

            // Remove deleted attributes
            var toRemove = existing.Attributes
                                   .Where(a => !variant.Attributes
                                                      .Any(x => x.VariantOptionValueId == a.VariantOptionValueId))
                                   .ToList();
            _db.RemoveRange(toRemove);

            // Add or update remaining
            existing.Attributes = variant.Attributes;

            _db.Entry(existing).CurrentValues.SetValues(variant);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int variantId)
        {
            var variant = await _db.ProductVariants.FindAsync(variantId);
            if (variant != null)
            {
                _db.ProductVariants.Remove(variant);
                await _db.SaveChangesAsync();
            }
        }
    }
}
