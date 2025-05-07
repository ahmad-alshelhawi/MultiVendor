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
    public class ProductVariantImageRepository : IProductVariantImageRepository
    {
        private readonly AppDbContext _db;
        public ProductVariantImageRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(ProductVariantImage image)
        {
            _db.Set<ProductVariantImage>().Add(image);
            await _db.SaveChangesAsync();
        }
    }
}
