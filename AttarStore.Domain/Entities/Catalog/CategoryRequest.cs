using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Entities.Catalog
{
    public enum RequestStatus { Pending, Approved, Rejected }

    public class CategoryRequest
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Pending;
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
