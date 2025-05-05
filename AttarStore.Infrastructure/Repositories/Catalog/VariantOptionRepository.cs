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
    public class VariantOptionRepository : IVariantOptionRepository
    {
        private readonly AppDbContext _db;

        public VariantOptionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<VariantOption>> GetAllOptionsAsync()
        {
            return await _db.VariantOptions
                            .Include(o => o.Values)
                            .ToListAsync();
        }

        public async Task<IEnumerable<VariantOptionValue>> GetValuesByOptionAsync(int optionId)
        {
            return await _db.VariantOptionValues
                            .AsNoTracking()
                            .Where(v => v.VariantOptionId == optionId)
                            .ToListAsync();
        }

        public async Task<VariantOption> AddOptionAsync(VariantOption option)
        {
            _db.VariantOptions.Add(option);
            await _db.SaveChangesAsync();
            return option;
        }

        public async Task<VariantOptionValue> AddOptionValueAsync(VariantOptionValue value)
        {
            _db.VariantOptionValues.Add(value);
            await _db.SaveChangesAsync();
            return value;
        }
    }
}
