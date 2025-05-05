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
        /// <summary>
        /// Get all the variant option types (e.g. Color, Size, Capacity).
        /// </summary>
        Task<IEnumerable<VariantOption>> GetAllOptionsAsync();

        /// <summary>
        /// Get all the values for a given option (e.g. all the Size values).
        /// </summary>
        Task<IEnumerable<VariantOptionValue>> GetValuesByOptionAsync(int optionId);

        /// <summary>
        /// Add a new option type (e.g. a new VariantOption).
        /// </summary>
        Task<VariantOption> AddOptionAsync(VariantOption option);

        /// <summary>
        /// Add a new value under an existing option.
        /// </summary>
        Task<VariantOptionValue> AddOptionValueAsync(VariantOptionValue value);
    }
}
