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
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _db;
        public ProductImageRepository(AppDbContext db) => _db = db;

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId)
        {
            return await _db.ProductImages
                .Where(img => img.ProductId == productId)
                .ToArrayAsync();
        }

        public async Task AddAsync(ProductImage image)
        {
            _db.ProductImages.Add(image);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int imageId)
        {
            var image = await _db.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                _db.ProductImages.Remove(image);
                await _db.SaveChangesAsync();
            }
        }
    }
}
