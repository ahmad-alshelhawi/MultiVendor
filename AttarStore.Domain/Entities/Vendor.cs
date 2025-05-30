﻿using AttarStore.Domain.Entities.Catalog;
using System;
using System.Collections.Generic;

namespace AttarStore.Domain.Entities
{
    public class Vendor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        // Vendor’s users
        public ICollection<User> Users { get; set; }
            = new List<User>();
        public ICollection<Product> Products { get; set; }
            = new List<Product>();
        public ICollection<CategoryRequest> CategoryRequests { get; set; }
            = new List<CategoryRequest>();
        public ICollection<VendorNotification> VendorNotifications { get; set; }
            = new List<VendorNotification>();
    }
}
