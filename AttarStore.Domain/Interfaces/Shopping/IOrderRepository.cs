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
        Task<Order[]> GetAllAsync();
        Task<Order[]> GetByClientAsync(int clientId);
        Task<Order> GetByIdAsync(int id);

        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateStatusAsync(int orderId, string status);
        Task DeleteAsync(int id);
    }
}
