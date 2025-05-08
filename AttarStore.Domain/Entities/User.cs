using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Entities.Shopping;
using AttarStore.Domain.Entities.submodels;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AttarStore.Domain.Entities
{
    public class User : IUser
    {
        public int Id { get; set; }

        // Profile
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; } = "";

        // Role & optional vendor/admin links
        public string Role { get; set; }
        public int? VendorId { get; set; }
        public Vendor? Vendor { get; set; }
        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }

        // Account state
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Password reset
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }

        // Refresh tokens
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        // ─── NEW: per-user permission overrides
        public ICollection<UserPermission> UserPermissions { get; set; }
            = new List<UserPermission>();

        public ICollection<InventoryTransaction> InventoryTransactions { get; set; }
             = new List<InventoryTransaction>();
        public ICollection<AuditLog> AuditLogs { get; set; }
            = new List<AuditLog>();





    }
}
