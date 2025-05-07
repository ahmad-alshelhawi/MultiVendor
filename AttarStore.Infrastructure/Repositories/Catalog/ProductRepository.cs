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
        // 1) save the bare product + its mapped variants
        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        // 2) if *no* variants were passed, create single default variant
        if (!product.Variants.Any())
        {
            var def = new ProductVariant
            {
                ProductId = product.Id,
                Sku = $"P{product.Id}-DEF",
                Price = product.DefaultPrice,
                Stock = product.DefaultStock
            };
            _db.ProductVariants.Add(def);
            await _db.SaveChangesAsync();
            return;
        }

        // 3) otherwise back-fill any zeros
        foreach (var v in product.Variants)
        {
            if (v.Price <= 0) v.Price = product.DefaultPrice;
            if (v.Stock <= 0) v.Stock = product.DefaultStock;
        }
        await _db.SaveChangesAsync();
    }

    // And mirror the same logic in UpdateAsync:
    public async Task UpdateAsync(Product product)
    {
        _db.Entry(product).State = EntityState.Modified;
        await _db.SaveChangesAsync();

        // back-fill defaults on existing variants
        if (!product.Variants.Any())
        {
            var def = new ProductVariant
            {
                ProductId = product.Id,
                Sku = $"P{product.Id}-DEF",
                Price = product.DefaultPrice,
                Stock = product.DefaultStock
            };
            _db.ProductVariants.Add(def);
            await _db.SaveChangesAsync();
            return;
        }
        else
        {
            foreach (var v in product.Variants)
            {
                if (v.Price <= 0) v.Price = product.DefaultPrice;
                if (v.Stock <= 0) v.Stock = product.DefaultStock;
            }
            await _db.SaveChangesAsync();
        }
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