using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Interfaces.Catalog;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;

public class VariantOptionRepository : IVariantOptionRepository
{
    private readonly AppDbContext _db;
    public VariantOptionRepository(AppDbContext db) => _db = db;

    public Task<VariantOption[]> GetAllOptionsAsync()
        => _db.VariantOptions.ToArrayAsync();

    public Task<VariantOption> GetOptionByIdAsync(int id)
        => _db.VariantOptions.FindAsync(id).AsTask();

    public async Task AddOptionAsync(VariantOption opt)
    {
        _db.VariantOptions.Add(opt);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteOptionAsync(int id)
    {
        var o = await _db.VariantOptions.FindAsync(id);
        if (o != null) { _db.VariantOptions.Remove(o); await _db.SaveChangesAsync(); }
    }

    public Task<VariantOptionValue[]> GetValuesByOptionAsync(int optionId)
        => _db.VariantOptionValues
              .Where(v => v.VariantOptionId == optionId)
              .ToArrayAsync();

    public async Task AddValueAsync(VariantOptionValue v)
    {
        _db.VariantOptionValues.Add(v);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteValueAsync(int id)
    {
        var v = await _db.VariantOptionValues.FindAsync(id);
        if (v != null) { _db.VariantOptionValues.Remove(v); await _db.SaveChangesAsync(); }
    }
}