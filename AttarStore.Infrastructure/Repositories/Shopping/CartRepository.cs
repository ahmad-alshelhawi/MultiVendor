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
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _db;
        public CartRepository(AppDbContext db) => _db = db;

        public async Task<Cart> GetByClientIdAsync(int clientId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.ProductVariant)
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (cart != null) return cart;

            cart = new Cart { ClientId = clientId };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();
            return cart;
        }

        public async Task<CartItem> AddItemAsync(int clientId, int variantId, int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");

            var cart = await GetByClientIdAsync(clientId);

            // snapshot price
            var variant = await _db.ProductVariants
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == variantId);
            if (variant == null) throw new InvalidOperationException("Variant not found.");

            var existing = cart.Items.SingleOrDefault(i => i.ProductVariantId == variantId);
            if (existing != null)
            {
                existing.Quantity += quantity;
                _db.Entry(existing).State = EntityState.Modified;
            }
            else
            {
                existing = new CartItem
                {
                    CartId = cart.Id,
                    ProductVariantId = variantId,
                    Quantity = quantity,
                    UnitPrice = variant.Price
                };
                _db.CartItems.Add(existing);
            }

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<CartItem> UpdateItemAsync(int itemId, int quantity)
        {
            var item = await _db.CartItems.FindAsync(itemId);
            if (item == null) throw new InvalidOperationException("Cart item not found.");
            if (quantity <= 0) throw new ArgumentException("Quantity must be positive.");

            item.Quantity = quantity;
            _db.Entry(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task RemoveItemAsync(int itemId)
        {
            var item = await _db.CartItems.FindAsync(itemId);
            if (item != null)
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int clientId)
        {
            var cart = await GetByClientIdAsync(clientId);
            _db.CartItems.RemoveRange(cart.Items);
            await _db.SaveChangesAsync();
        }
    }
}
