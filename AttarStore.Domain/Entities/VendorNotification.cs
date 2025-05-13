using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Entities
{
    public class VendorNotification
    {
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; } = null!;
        public int NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;
        public bool IsRead { get; set; }
    }
}
