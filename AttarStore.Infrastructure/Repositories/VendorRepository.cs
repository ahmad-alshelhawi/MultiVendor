using AttarStore.Domain.Entities;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AttarStore.Services.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly AppDbContext _db;

        public VendorRepository(AppDbContext dbContext)
            => _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<bool> ExistsByNameOrContactEmailAsync(
            string name,
            string contactEmail,
            int? excludeVendorId = null)
        {
            // Vendors
            var vendorExists = await _db.Vendors.AnyAsync(v =>
                (v.Name == name || v.ContactEmail == contactEmail)
                && (!excludeVendorId.HasValue || v.Id != excludeVendorId.Value));

            // Admins
            var adminExists = await _db.Admins.AnyAsync(a =>
                !a.IsDeleted
                && (a.Name == name || a.Email == contactEmail));

            // Users
            var userExists = await _db.Users.AnyAsync(u =>
                !u.IsDeleted
                && (u.Name == name || u.Email == contactEmail));

            // Clients
            var clientExists = await _db.Clients.AnyAsync(c =>
                (c.Name == name || c.Email == contactEmail));

            return vendorExists || adminExists || userExists || clientExists;
        }

        public async Task<Vendor[]> GetAllAsync()
            => await _db.Vendors
                .AsNoTracking()
                .Where(v => v.IsActive)
                .ToArrayAsync();

        public async Task<Vendor> GetByIdAsync(int id)
            => await _db.Vendors
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);

        public async Task<Vendor> GetByNameAsync(string name)
            => await _db.Vendors
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Name == name && v.IsActive);

        public async Task<Vendor> GetByContactEmailAsync(string email)
            => await _db.Vendors
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.ContactEmail == email && v.IsActive);

        public async Task AddAsync(Vendor vendor)
        {
            if (vendor is null)
                throw new ArgumentNullException(nameof(vendor));

            if (await ExistsByNameOrContactEmailAsync(vendor.Name, vendor.ContactEmail))
                throw new InvalidOperationException("Vendor name or contact email already in use.");

            _db.Vendors.Add(vendor);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vendor vendor)
        {
            if (vendor is null)
                throw new ArgumentNullException(nameof(vendor));

            if (await ExistsByNameOrContactEmailAsync(
                vendor.Name, vendor.ContactEmail, vendor.Id))
            {
                throw new InvalidOperationException("Vendor name or contact email already in use.");
            }

            _db.Entry(vendor).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var vendor = await _db.Vendors.FindAsync(id);
            if (vendor != null)
            {
                vendor.IsActive = false;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> SaveChangesAsync()
            => await _db.SaveChangesAsync() > 0;

        public async Task<IEnumerable<User>> GetUsersByVendorIdAsync(int vendorId)
        {
            // If your User entity has a VendorId FK:
            return await _db.Users
                             .Where(u => u.VendorId == vendorId)
                             .ToListAsync();

            // Or, if you prefer navigating from Vendor:
            // var vendor = await _ctx.Vendors
            //                        .Include(v => v.Users)
            //                        .FirstOrDefaultAsync(v => v.Id == vendorId);
            // return vendor?.Users ?? Enumerable.Empty<User>();
        }
        public async Task<IEnumerable<User>> GetAssignedUsersAsync(int vendorId)
            => await _db.Users
                         .Where(u => u.VendorId == vendorId
                                  && (u.Role == Roles.VendorAdmin || u.Role == Roles.VendorUser))
                         .ToListAsync();
    }
}
