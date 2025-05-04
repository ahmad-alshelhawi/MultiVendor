using AttarStore.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Services.Interfaces.Catalog
{
    public interface ISubcategoryRepository
    {
        Task<Subcategory[]> GetByCategoryIdAsync(int categoryId);
        Task<Subcategory> GetByIdAsync(int id);

        Task AddAsync(Subcategory subcategory);
        Task UpdateAsync(Subcategory subcategory);
        Task DeleteAsync(int id);
    }
}