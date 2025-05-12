using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Entities.Auth
{
    public class Notification
    {
        public int Id { get; set; }
        public int? UserId { get; set; }            // null => broadcast to all
        public string? TargetRole { get; set; }     // null => not role-specific
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
