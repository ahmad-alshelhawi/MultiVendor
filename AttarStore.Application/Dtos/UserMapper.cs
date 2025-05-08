using System.ComponentModel.DataAnnotations;

namespace AttarStore.Application.Dtos
{
    // ─── View Model ─────────────────────────────────────────────────────────
    public class UserMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public int? VendorId { get; set; }
    }

    // ─── Create DTO ───────────────────────────────────────────────────────────
    public class UserMapperCreate
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string? Role { get; set; }

    }

    // ─── Update DTO ───────────────────────────────────────────────────────────
    public class UserMapperUpdate
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; }

    }

    // ─── Profile‐Update DTO ───────────────────────────────────────────────────
    public class UserProfileUpdateMapper
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    // ─── Change‐Password DTO ──────────────────────────────────────────────────
    public class ChangePasswordUser
    {
        [Required] public string CurrentPassword { get; set; }
        [Required] public string NewPassword { get; set; }
    }


    // ─── VendorUserCreate DTO ──────────────────────────────────────────────────

    public class VendorUserCreate
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
    public class AdminUserMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public int? AdminId { get; set; }
    }

}
