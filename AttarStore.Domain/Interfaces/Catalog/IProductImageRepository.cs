using AttarStore.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces.Catalog
{
    public interface IProductImageRepository
    {
        Task AddAsync(ProductImage image);
        Task DeleteAsync(int imageId);
    }
}
