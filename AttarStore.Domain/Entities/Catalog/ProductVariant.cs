using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Entities.Shopping;
using System.ComponentModel.DataAnnotations;

public class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }

    // NEW: SKU, price and stock on a per-variant basis
    public string Sku { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public ICollection<ProductVariantImage> Images { get; set; } = new List<ProductVariantImage>();
    public ICollection<ProductVariantAttribute> Attributes { get; set; } = new List<ProductVariantAttribute>();
    public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
       = new List<InventoryTransaction>();
}
