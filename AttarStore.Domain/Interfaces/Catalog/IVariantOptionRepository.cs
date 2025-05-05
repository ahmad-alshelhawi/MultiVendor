using AttarStore.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces.Catalog
{
    public interface IVariantOptionRepository
    {
        Task<VariantOption[]> GetAllOptionsAsync();
        Task<VariantOption> GetOptionByIdAsync(int id);
        Task AddOptionAsync(VariantOption opt);
        Task DeleteOptionAsync(int id);

        Task<VariantOptionValue[]> GetValuesByOptionAsync(int optionId);
        Task AddValueAsync(VariantOptionValue val);
        Task DeleteValueAsync(int id);
    }
}
