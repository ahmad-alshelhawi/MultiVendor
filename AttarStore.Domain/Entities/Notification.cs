﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
        public ICollection<AdminNotification> AdminNotifications { get; set; } = new List<AdminNotification>();
        public ICollection<ClientNotification> ClientNotifications { get; set; } = new List<ClientNotification>();
        public ICollection<VendorNotification> VendorNotifications { get; set; } = new List<VendorNotification>();
    }
}
