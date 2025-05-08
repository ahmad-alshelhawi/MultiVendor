// Domain/Entities/Auth/AuditLog.cs
using System;

namespace AttarStore.Domain.Entities.Auth
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int ActorId { get; set; }
        public string ActorType { get; set; } = null!;
        public string ActorName { get; set; } = null!;
        public string ActorRole { get; set; } = null!;

        public string Action { get; set; } = null!;
        public string EntityType { get; set; } = null!;
        public int? EntityId { get; set; }
        public string? EntityName { get; set; }    // ← NEW
        public DateTime Timestamp { get; set; }
        public string? Details { get; set; }
    }
}
