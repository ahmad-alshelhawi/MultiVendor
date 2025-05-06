using AttarStore.Domain.Entities.Shopping;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Repositories.Shopping
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly AppDbContext _db;
        public InventoryTransactionRepository(AppDbContext db) => _db = db;

        /// <inheritdoc />
        public async Task<IEnumerable<InventoryTransaction>> GetAllAsync()
        {
            return await _db.InventoryTransactions
                            .Include(tx => tx.User)
                            .OrderByDescending(tx => tx.Timestamp)
                            .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InventoryTransaction>> GetByFilterAsync(int? variantId, int? productId)
        {
            var query = _db.InventoryTransactions.AsQueryable();

            if (variantId.HasValue)
                query = query.Where(tx => tx.ProductVariantId == variantId.Value);
            else if (productId.HasValue)
                query = query.Where(tx => tx.ProductId == productId.Value);

            return await query
                .Include(tx => tx.User)
                .OrderByDescending(tx => tx.Timestamp)
                .ToArrayAsync();
        }

        /// <inheritdoc />
        public async Task AddAsync(InventoryTransaction transaction)
        {
            _db.InventoryTransactions.Add(transaction);
            await _db.SaveChangesAsync();
        }
    }
}
