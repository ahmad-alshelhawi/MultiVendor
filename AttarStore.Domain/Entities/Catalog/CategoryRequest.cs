// AttarStore.Domain.Entities.Catalog/CategoryRequest.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AttarStore.Domain.Entities.Catalog
{
    public enum CategoryRequestStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class CategoryRequest
    {
        public int Id { get; set; }

        // Who asked
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public CategoryRequestStatus Status { get; set; } = CategoryRequestStatus.Pending;

        /// <summary>
        /// Admin’s optional note back to vendor
        /// </summary>
        public string? ResponseMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
