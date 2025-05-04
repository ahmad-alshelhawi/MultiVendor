using System;
using System.ComponentModel.DataAnnotations;

namespace AttarStore.Application.Dtos
{
    // ─── View Model ─────────────────────────────────────────────────────────
    public class AdminMapperView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    // ─── Create DTO ───────────────────────────────────────────────────────────
    public class AdminMapperCreate
    {
        [Required] public string Name { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string Phone { get; set; }
        public string Address { get; set; }
    }

    // ─── Update DTO ───────────────────────────────────────────────────────────
    public class AdminMapperUpdate
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    // ─── Profile‐Update DTO ───────────────────────────────────────────────────
    public class AdminProfileUpdateMapper
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    // ─── Change‐Password DTO ──────────────────────────────────────────────────
    public class ChangePasswordAdmin
    {
        [Required] public string CurrentPassword { get; set; }
        [Required] public string NewPassword { get; set; }
    }
}
