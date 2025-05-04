using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces.Catalog
{
    public interface IProductRepository
    {
        Task<Product[]> GetAllAsync();
        Task<Product> GetByIdAsync(int id);

        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
