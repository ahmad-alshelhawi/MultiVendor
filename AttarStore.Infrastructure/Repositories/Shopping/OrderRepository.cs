using AttarStore.Domain.Entities.Shopping;
using AttarStore.Domain.Interfaces.Shopping;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Repositories.Shopping
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db) => _db = db;

        public async Task<Order[]> GetAllAsync()
            => await _db.Orders
                .Include(o => o.Items)
                .AsNoTracking()
                .ToArrayAsync();

        public async Task<Order[]> GetByClientAsync(int clientId)
            => await _db.Orders
                .Where(o => o.ClientId == clientId)
                .Include(o => o.Items)
                .AsNoTracking()
                .ToArrayAsync();

        public async Task<Order> GetByIdAsync(int id)
            => await _db.Orders
                .Include(o => o.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<Order> CreateAsync(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            // snapshot each item's unit price
            foreach (var item in order.Items)
            {
                var variant = await _db.ProductVariants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.Id == item.ProductVariantId);
                if (variant == null)
                    throw new InvalidOperationException($"Variant {item.ProductVariantId} not found.");
                item.UnitPrice = variant.Price;
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null) throw new InvalidOperationException("Order not found.");
            order.Status = status;
            _db.Entry(order).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order != null)
            {
                _db.Orders.Remove(order);
                await _db.SaveChangesAsync();
            }
        }
    }
}
