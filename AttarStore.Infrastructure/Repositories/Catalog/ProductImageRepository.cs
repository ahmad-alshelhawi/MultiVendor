using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AttarStore.Services.Data;
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

        public async Task<ProductImage> AddAsync(ProductImage image)
        {
            _db.ProductImages.Add(image);
            await _db.SaveChangesAsync();
            return image;
        }
    }
}
