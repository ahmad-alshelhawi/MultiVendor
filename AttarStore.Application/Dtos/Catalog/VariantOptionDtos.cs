// in AttarStore.Application/Dtos/Catalog/VariantOptionDtos.cs
using System.ComponentModel.DataAnnotations;

namespace AttarStore.Application.Dtos.Catalog
{
    /// <summary>
    /// Used when creating a new option (e.g. “Color”, “Size”, etc.)
    /// </summary>
    public class VariantOptionCreateDto
    {
        [Required]
        public string Name { get; set; }
    }

    /// <summary>
    /// Used when creating a new value under an existing option
    /// e.g. for option “Color” you might have values “Red”, “Blue”, etc.
    /// </summary>
    public class VariantOptionValueCreateDto
    {
        [Required]
        public int OptionId { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
