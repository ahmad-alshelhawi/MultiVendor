using System;
using System.Collections.Generic;
using AttarStore.Domain.Entities.Auth;

namespace AttarStore.Domain.Entities.submodels
{
    /// <summary>
    /// Common properties for all account‐holding actors (Admin, User, Client, VendorUser, etc.).
    /// </summary>
    public interface IUser
    {
        int Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string Phone { get; set; }
        string Address { get; set; }
        string Role { get; set; }
        string? ResetToken { get; set; }
        DateTime? ResetTokenExpiry { get; set; }
        bool IsDeleted { get; set; }
        DateTimeOffset CreatedAt { get; set; }

        // Navigation for refresh tokens (shared across actors)
        ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
