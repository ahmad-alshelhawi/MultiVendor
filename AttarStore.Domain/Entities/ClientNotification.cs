using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Entities
{
    public class ClientNotification
    {
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public int NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;
        public bool IsRead { get; set; }
    }
}
