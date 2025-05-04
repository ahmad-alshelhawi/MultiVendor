using System;

namespace AttarStore.Domain.Entities.Auth
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }    // “Admin”, “Customer”, etc.

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }

        public int? ClientId { get; set; }
        public Client? Client { get; set; }

        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
    }
}
