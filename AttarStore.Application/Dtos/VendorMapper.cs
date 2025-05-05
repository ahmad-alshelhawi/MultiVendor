using System.ComponentModel.DataAnnotations;

namespace AttarStore.Application.Dtos
{
    // ─── View Model ─────────────────────────────────────────────────────────
    public class VendorMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    // ─── Create DTO ───────────────────────────────────────────────────────────
    public class VendorMapperCreate
    {
        [Required] public string Name { get; set; }
        [Required] public string ContactEmail { get; set; }
        [Required] public string Phone { get; set; }
        public string Address { get; set; }
    }

    // ─── Update DTO ───────────────────────────────────────────────────────────
    public class VendorMapperUpdate
    {
        public string? Name { get; set; }
        public string? ContactEmail { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    // ─── Profile‐Update DTO ───────────────────────────────────────────────────
    public class VendorProfileUpdateMapper
    {
        public string? Name { get; set; }
        public string? ContactEmail { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }


}
