using AttarStore.Domain.Entities.Shopping;
using AttarStore.Domain.Interfaces.Shopping;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Repositories.Shopping
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db) => _db = db;

        public async Task<Order> GetByIdAsync(int id) =>
            await _db.Orders
                     .Include(o => o.Items)
                     .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<Order[]> GetAllForClientAsync(int clientId) =>
            await _db.Orders
                     .Where(o => o.ClientId == clientId)
                     .Include(o => o.Items)
                     .ToArrayAsync();

        public async Task<Order> CreateAsync(Order order)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<Order> CheckoutAsync(int clientId)
        {
            // 1) load cart + items
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
                // a) decrement stock
                ci.ProductVariant.Stock -= ci.Quantity;
                // b) persist inventory transaction
                _db.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductVariantId = ci.ProductVariantId,
                    ProductId = null,
                    Quantity = -ci.Quantity,
                    Reason = "Checkout",
                    UserId = clientId,
                    Timestamp = DateTime.UtcNow
                });

                // c) attach to order
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

            // 6) return populated order
            return await GetByIdAsync(order.Id);
        }
    }
}

