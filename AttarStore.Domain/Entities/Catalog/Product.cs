using AttarStore.Domain.Entities.Catalog;
using AttarStore.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Details { get; set; }
    public int? SubcategoryId { get; set; }
    public Subcategory? Subcategory { get; set; }

    public int VendorId { get; set; }
    public Vendor Vendor { get; set; }

    public ICollection<ProductImage> Images { get; set; }
        = new List<ProductImage>();
    public ICollection<ProductVariant> Variants { get; set; }
        = new List<ProductVariant>();
}