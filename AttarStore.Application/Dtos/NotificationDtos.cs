using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Application.Dtos
{
    public class CreateNotificationDto
    {
        public int? UserId { get; set; }    // specify for single-user
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
    public class NotificationDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? TargetRole { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
