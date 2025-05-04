using System.ComponentModel.DataAnnotations;

namespace AttarStore.Application.Dtos
{
    // ─── View Model ─────────────────────────────────────────────────────────
    public class ClientMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    // ─── Create DTO ───────────────────────────────────────────────────────────
    public class ClientMapperCreate
    {
        [Required] public string Name { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string Phone { get; set; }
        public string Address { get; set; }
    }

    // ─── Update DTO ───────────────────────────────────────────────────────────
    public class ClientMapperUpdate
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    // ─── Profile‐Update DTO ───────────────────────────────────────────────────
    public class ClientProfileUpdateMapper
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    // ─── Change‐Password DTO ──────────────────────────────────────────────────
    public class ChangePasswordClient
    {
        [Required] public string CurrentPassword { get; set; }
        [Required] public string NewPassword { get; set; }
    }
}
