// AttarStore.Application/Dtos/Catalog/CategoryRequestDtos.cs
using System;
using System.ComponentModel.DataAnnotations;
using AttarStore.Domain.Entities.Catalog;

namespace AttarStore.Application.Dtos.Catalog
{
    /// <summary>
    /// Vendor → create a new category request
    /// </summary>
    public class CategoryRequestCreateDto
    {
        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }

    /// <summary>
    /// Shared view model for listing requests
    /// </summary>
    public class CategoryRequestDto
    {
        public int Id { get; set; }

        public int VendorId { get; set; }

        public string VendorName { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public CategoryRequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Admin → approve or reject a request
    /// </summary>
    public class CategoryRequestUpdateDto
    {
        [Required]
        public CategoryRequestStatus Status { get; set; }

        /// <summary>
        /// Optional note back to the vendor
        /// </summary>
        public string? ResponseMessage { get; set; }
    }
}
