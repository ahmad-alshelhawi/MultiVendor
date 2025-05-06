using AttarStore.Domain.Interfaces;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Services
{
    public class VendorService : IVendorService
    {
        private readonly AppDbContext _db;
        public VendorService(AppDbContext db) => _db = db;

        public async Task SuspendVendorAsync(int vendorId)
        {
            var vendor = await _db.Vendors
                .Include(v => v.Users)
                .FirstOrDefaultAsync(v => v.Id == vendorId);
            if (vendor == null) throw new KeyNotFoundException($"Vendor {vendorId} not found");

            vendor.IsActive = false;
            foreach (var u in vendor.Users)
                u.IsActive = false;

            await _db.SaveChangesAsync();
        }

        public async Task ActivateVendorAsync(int vendorId)
        {
            var vendor = await _db.Vendors
                .Include(v => v.Users)
                .FirstOrDefaultAsync(v => v.Id == vendorId);
            if (vendor == null) throw new KeyNotFoundException($"Vendor {vendorId} not found");

            vendor.IsActive = true;
            foreach (var u in vendor.Users)
                u.IsActive = true;

            await _db.SaveChangesAsync();
        }
    }
}
