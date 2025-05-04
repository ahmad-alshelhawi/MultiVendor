using System.Threading.Tasks;
using AttarStore.Domain.Entities.Catalog;


namespace AttarStore.Domain.Interfaces.Catalog
{
    public interface ICategoryRepository
    {
        Task<Category[]> GetAllAsync();
        Task<Category> GetByIdAsync(int id);

        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}
