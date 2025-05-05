using AttarStore.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces.Catalog
{
    public interface IProductVariantRepository
    {
        /// <summary>
        /// Get all variants for a given product.
        /// </summary>
        Task<IEnumerable<ProductVariant>> GetByProductIdAsync(int productId);

        /// <summary>
        /// Get a specific variant by its ID.
        /// </summary>
        Task<ProductVariant> GetByIdAsync(int variantId);

        /// <summary>
        /// Add a new variant (with its attributes).
        /// </summary>
        Task<ProductVariant> AddAsync(ProductVariant variant);

        /// <summary>
        /// Update an existing variant (and its attributes).
        /// </summary>
        Task UpdateAsync(ProductVariant variant);

        /// <summary>
        /// Delete a variant by ID.
        /// </summary>
        Task DeleteAsync(int variantId);
    }
}
