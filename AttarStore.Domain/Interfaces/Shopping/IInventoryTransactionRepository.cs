using AttarStore.Domain.Entities.Shopping;

public interface IInventoryTransactionRepository
{
    /// <summary>
    /// Returns all inventory transactions (across all products & variants).
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetAllAsync();

    /// <summary>
    /// Returns transactions for a specific variant (if variantId != null)
    /// or base product (if productId != null and variantId == null).
    /// </summary>
    Task<IEnumerable<InventoryTransaction>> GetByFilterAsync(int? variantId, int? productId);

    /// <summary>
    /// Append a new inventory transaction.
    /// </summary>
    Task AddAsync(InventoryTransaction transaction);
}
