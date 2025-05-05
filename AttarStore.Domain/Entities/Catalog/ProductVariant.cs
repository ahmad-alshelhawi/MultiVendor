using AttarStore.Domain.Entities.Catalog;

public class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public string Sku { get; set; }
    public decimal Price { get; set; }

    public int Stock { get; set; }

    public ICollection<ProductVariantAttribute> Attributes { get; set; }
        = new List<ProductVariantAttribute>();
}