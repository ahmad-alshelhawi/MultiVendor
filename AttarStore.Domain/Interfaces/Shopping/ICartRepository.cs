using AttarStore.Domain.Entities.Shopping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces.Shopping
{
    public interface ICartRepository
    {
        /// <summary> Get or create the cart for this client. </summary>
        Task<Cart> GetByClientIdAsync(int clientId);

        /// <summary> Add a new item (or increment quantity if exists). </summary>
        Task<CartItem> AddItemAsync(int clientId, int variantId, int quantity);

        /// <summary> Update the quantity of an existing item. </summary>
        Task<CartItem> UpdateItemAsync(int itemId, int quantity);

        /// <summary> Remove a single item. </summary>
        Task RemoveItemAsync(int itemId);

        /// <summary> Clear all items in the cart. </summary>
        Task ClearCartAsync(int clientId);
    }
}
