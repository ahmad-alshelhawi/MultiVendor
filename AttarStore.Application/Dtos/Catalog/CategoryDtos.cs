using System.ComponentModel.DataAnnotations;

namespace AttarStore.Application.Dtos.Catalog
{
    // ─── Category ─────────────────────────────────────────────────────────────
    public class CategoryMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }

    public class CategoryMapperCreate
    {
        [Required] public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }

    public class CategoryMapperUpdate
    {
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }

    // ─── Subcategory ──────────────────────────────────────────────────────────
    public class SubcategoryMapperView
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }

    public class SubcategoryMapperCreate
    {
        [Required] public int CategoryId { get; set; }
        [Required] public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
    }

    public class SubcategoryMapperUpdate
    {
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }
}
