using AttarStore.Domain.Entities.Shopping;
using AttarStore.Domain.Interfaces.Shopping;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Repositories.Shopping
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db) => _db = db;

        // ─── now includes Items → ProductVariant → Product ─────────────────────
        public async Task<Order> GetByIdAsync(int id) =>
            await _db.Orders
                     .Where(o => o.Id == id)
                     .Include(o => o.Items)
                       .ThenInclude(oi => oi.ProductVariant)
                         .ThenInclude(pv => pv.Product)
                     .FirstOrDefaultAsync();

        // ─── same for list endpoint ───────────────────────────────────────────
        public async Task<Order[]> GetAllForClientAsync(int clientId) =>
            await _db.Orders
                     .Where(o => o.ClientId == clientId)
                     .Include(o => o.Items)
                       .ThenInclude(oi => oi.ProductVariant)
                         .ThenInclude(pv => pv.Product)
                     .ToArrayAsync();

        public async Task<Order> CreateAsync(Order order)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<Order> CheckoutAsync(int clientId)
        {
            // 1) load cart + items (already includes ProductVariant)
            var cart = await _db.Carts
                .Include(c => c.Items)
                   .ThenInclude(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (cart == null || !cart.Items.Any())
                throw new InvalidOperationException("Cart is empty.");

            // 2) build new order
            var order = new Order
            {
                ClientId = clientId,
                CreatedAt = DateTime.UtcNow
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // 3) move each cart‐item → order‐item, adjust stock, record inventory tx
            foreach (var ci in cart.Items)
            {
                ci.ProductVariant.Stock -= ci.Quantity;
                _db.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductVariantId = ci.ProductVariantId,
                    ProductId = null,
                    Quantity = -ci.Quantity,
                    Reason = "Checkout",
                    UserId = clientId,
                    Timestamp = DateTime.UtcNow
                });

                _db.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductVariantId = ci.ProductVariantId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.ProductVariant.Price
                });
            }

            // 4) clear the cart
            _db.CartItems.RemoveRange(cart.Items);

            // 5) save everything
            await _db.SaveChangesAsync();

            // 6) return fully‐populated order
            return await GetByIdAsync(order.Id);
        }
    }
}
