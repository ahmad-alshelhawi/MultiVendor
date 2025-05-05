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
                        .Include(p => p.Images)
                        .Include(p => p.Variants)
                            .ThenInclude(v => v.Attributes)
                        .AsNoTracking()
                        .ToArrayAsync();

        public async Task<Product> GetByIdAsync(int id)
            => await _db.Products
                        .Include(p => p.Images)
                        .Include(p => p.Variants)
                            .ThenInclude(v => v.Attributes)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Product product)
        {
            // 1) Detach variants
            var variants = product.Variants.ToList();
            product.Variants.Clear();

            // 2) Save the product itself
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            // 3) For each variant, detach its attributes, save variant, then save attributes
            foreach (var variant in variants)
            {
                var attrs = variant.Attributes.ToList();
                variant.Attributes.Clear();

                variant.ProductId = product.Id;
                _db.ProductVariants.Add(variant);
                await _db.SaveChangesAsync();

                foreach (var a in attrs)
                {
                    a.ProductVariantId = variant.Id;
                    _db.ProductVariantAttributes.Add(a);
                }
                await _db.SaveChangesAsync();
            }

            // 4) Optionally re-attach for in-memory consistency
            product.Variants = variants;
        }

        public async Task UpdateAsync(Product product)
        {
            // 1) Load existing variant+attributes
            var existing = await _db.ProductVariants
                                    .Include(v => v.Attributes)
                                    .Where(v => v.ProductId == product.Id)
                                    .ToListAsync();

            // 2) Determine removed variants
            var toRemoveVariants = existing
                .Where(ev => !product.Variants.Any(v => v.Id == ev.Id))
                .ToList();
            _db.ProductVariants.RemoveRange(toRemoveVariants);
            await _db.SaveChangesAsync();

            // 3) For each incoming variant:
            foreach (var varDto in product.Variants)
            {
                // New variant?
                if (varDto.Id == 0)
                {
                    // detach its attrs
                    var newAttrs = varDto.Attributes.ToList();
                    varDto.Attributes.Clear();

                    varDto.ProductId = product.Id;
                    _db.ProductVariants.Add(varDto);
                    await _db.SaveChangesAsync();

                    foreach (var a in newAttrs)
                    {
                        a.ProductVariantId = varDto.Id;
                        _db.ProductVariantAttributes.Add(a);
                    }
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // Existing variant: update scalar and sync attributes
                    var ev = existing.First(ev2 => ev2.Id == varDto.Id);
                    _db.Entry(ev).CurrentValues.SetValues(varDto);

                    // Remove attributes that were deleted
                    var toRemoveAttrs = ev.Attributes
                        .Where(ea => !varDto.Attributes
                            .Any(va => va.VariantOptionId == ea.VariantOptionId
                                    && va.VariantOptionValueId == ea.VariantOptionValueId))
                        .ToList();
                    _db.ProductVariantAttributes.RemoveRange(toRemoveAttrs);

                    // Add new attributes
                    var existingKeys = ev.Attributes
                        .Select(ea => (ea.VariantOptionId, ea.VariantOptionValueId))
                        .ToHashSet();
                    var toAddAttrs = varDto.Attributes
                        .Where(va => !existingKeys.Contains((va.VariantOptionId, va.VariantOptionValueId)))
                        .ToList();
                    foreach (var a in toAddAttrs)
                    {
                        a.ProductVariantId = ev.Id;
                        _db.ProductVariantAttributes.Add(a);
                    }
                }
            }

            // 4) Finally update product itself
            _db.Entry(product).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
            }
        }
    }
}
