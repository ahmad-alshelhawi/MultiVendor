using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;
    public ProductRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _db.Products
                    .Include(p => p.Images)
                    .Include(p => p.Variants)
                        .ThenInclude(v => v.Images)
                    .Include(p => p.Variants)
                        .ThenInclude(v => v.Attributes)
                            .ThenInclude(a => a.VariantOption)
                    .Include(p => p.Variants)
                        .ThenInclude(v => v.Attributes)
                            .ThenInclude(a => a.VariantOptionValue)
                    .Include(p => p.Subcategory)
                    .Include(p => p.Vendor)
                    .ToArrayAsync();

    public async Task<Product> GetByIdAsync(int id)
        => await _db.Products
                    .Include(p => p.Images)
                    .Include(p => p.Variants)
                        .ThenInclude(v => v.Images)
                    .Include(p => p.Variants)
                        .ThenInclude(v => v.Attributes)
                            .ThenInclude(a => a.VariantOption)
                    .Include(p => p.Variants)
                        .ThenInclude(v => v.Attributes)
                            .ThenInclude(a => a.VariantOptionValue)
                    .Include(p => p.Subcategory)
                    .Include(p => p.Vendor)
                    .FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _db.Entry(product).State = EntityState.Modified;
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Products.FindAsync(id);
        if (entity != null)
        {
            _db.Products.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}