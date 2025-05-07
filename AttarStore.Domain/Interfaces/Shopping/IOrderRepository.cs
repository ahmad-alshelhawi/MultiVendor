using AttarStore.Domain.Entities.Shopping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces.Shopping
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(int id);
        Task<Order[]> GetAllForClientAsync(int clientId);
        Task<Order> CreateAsync(Order order);

        // ← new:
        Task<Order> CheckoutAsync(int clientId);
    }

}
