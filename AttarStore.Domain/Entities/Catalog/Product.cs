using AttarStore.Domain.Entities.Catalog;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Details { get; set; }   // rich text/markdown

    public int? SubcategoryId { get; set; }
    public Subcategory Subcategory { get; set; }

    // Images & Variants
    public ICollection<ProductImage> Images { get; set; }
        = new List<ProductImage>();
    public ICollection<ProductVariant> Variants { get; set; }
        = new List<ProductVariant>();
}