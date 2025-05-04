using System;
using System.Linq;
using System.Threading.Tasks;
using AttarStore.Domain.Entities;
using AttarStore.Services.Data;
using AttarStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AttarStore.Services.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _db;

        public AdminRepository(AppDbContext dbContext)
            => _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<Admin> GetByUsernameAsync(string username)
            => await _db.Admins
                .AsNoTracking()
                .SingleOrDefaultAsync(a => !a.IsDeleted && a.Name == username);

        public async Task<Admin> GetByIdAsync(int id)
            => await _db.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => !a.IsDeleted && a.Id == id);

        public async Task<Admin> GetByEmailAsync(string email)
            => await _db.Admins
                .AsNoTracking()
                .FirstOrDefaultAsync(a => !a.IsDeleted && a.Email == email);

        public async Task<Admin[]> GetAllAsync()
            => await _db.Admins
                .AsNoTracking()
                .Where(a => !a.IsDeleted)
                .ToArrayAsync();

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty", nameof(email));

            return await _db.Admins.AnyAsync(a => a.Email == email && !a.IsDeleted);
        }

        public async Task AddAsync(Admin admin)
        {
            if (admin is null)
                throw new ArgumentNullException(nameof(admin));

            // Uniqueness across Admins, Users, Clients & Vendors
            bool nameTaken =
                await _db.Admins.AnyAsync(a => a.Name == admin.Name && !a.IsDeleted) ||
                await _db.Users.AnyAsync(u => u.Name == admin.Name && !u.IsDeleted) ||
                await _db.Clients.AnyAsync(c => c.Name == admin.Name && !c.IsDeleted) ||
                await _db.Vendors.AnyAsync(v => v.Name == admin.Name);

            if (nameTaken)
                throw new InvalidOperationException("Name already exists in the system.");

            bool emailTaken =
                await _db.Admins.AnyAsync(a => a.Email == admin.Email && !a.IsDeleted) ||
                await _db.Users.AnyAsync(u => u.Email == admin.Email && !u.IsDeleted) ||
                await _db.Clients.AnyAsync(c => c.Email == admin.Email && !c.IsDeleted) ||
                await _db.Vendors.AnyAsync(v => v.ContactEmail == admin.Email);

            if (emailTaken)
                throw new InvalidOperationException("Email already exists in the system.");

            _db.Admins.Add(admin);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Admin admin)
        {
            if (admin is null)
                throw new ArgumentNullException(nameof(admin));

            bool nameTaken =
                await _db.Admins.AnyAsync(a => a.Id != admin.Id && a.Name == admin.Name && !a.IsDeleted) ||
                await _db.Users.AnyAsync(u => u.Name == admin.Name && !u.IsDeleted) ||
                await _db.Clients.AnyAsync(c => c.Name == admin.Name && !c.IsDeleted) ||
                await _db.Vendors.AnyAsync(v => v.Name == admin.Name);

            if (nameTaken)
                throw new InvalidOperationException("Name already exists in the system.");

            bool emailTaken =
                await _db.Admins.AnyAsync(a => a.Id != admin.Id && a.Email == admin.Email && !a.IsDeleted) ||
                await _db.Users.AnyAsync(u => u.Email == admin.Email && !u.IsDeleted) ||
                await _db.Clients.AnyAsync(c => c.Email == admin.Email && !c.IsDeleted) ||
                await _db.Vendors.AnyAsync(v => v.ContactEmail == admin.Email);

            if (emailTaken)
                throw new InvalidOperationException("Email already exists in the system.");

            _db.Entry(admin).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var admin = await _db.Admins.FindAsync(id);
            if (admin != null)
            {
                admin.IsDeleted = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<string> UpdateProfileAsync(int adminId, string name, string phone, string email)
        {
            var admin = await GetByIdAsync(adminId);
            if (admin == null)
                return "Admin not found";

            bool isUpdated = false;

            // Name change
            if (!string.IsNullOrWhiteSpace(name) &&
                !name.Equals(admin.Name, StringComparison.OrdinalIgnoreCase))
            {
                bool nameExists =
                    await _db.Admins.AnyAsync(a => a.Id != adminId && a.Name == name && !a.IsDeleted) ||
                    await _db.Users.AnyAsync(u => u.Name == name && !u.IsDeleted) ||
                    await _db.Clients.AnyAsync(c => c.Name == name && !c.IsDeleted) ||
                    await _db.Vendors.AnyAsync(v => v.Name == name);

                if (nameExists)
                    return "Name already exists in the system.";

                admin.Name = name;
                isUpdated = true;
            }

            // Email change
            if (!string.IsNullOrWhiteSpace(email) &&
                !email.Equals(admin.Email, StringComparison.OrdinalIgnoreCase))
            {
                bool emailExists =
                    await _db.Admins.AnyAsync(a => a.Id != adminId && a.Email == email && !a.IsDeleted) ||
                    await _db.Users.AnyAsync(u => u.Email == email && !u.IsDeleted) ||
                    await _db.Clients.AnyAsync(c => c.Email == email && !c.IsDeleted) ||
                    await _db.Vendors.AnyAsync(v => v.ContactEmail == email);

                if (emailExists)
                    return "Email already exists in the system.";

                admin.Email = email;
                isUpdated = true;
            }

            // Phone change
            if (!string.IsNullOrWhiteSpace(phone) &&
                !phone.Equals(admin.Phone, StringComparison.OrdinalIgnoreCase))
            {
                admin.Phone = phone;
                isUpdated = true;
            }

            if (!isUpdated)
                return "No changes have been made.";

            _db.Entry(admin).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return "Profile updated successfully.";
        }

        public async Task<bool> UpdatePasswordAsync(int adminId, string currentPassword, string newPassword)
        {
            var admin = await GetByIdAsync(adminId);
            if (admin == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, admin.Password))
                return false;

            admin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _db.Entry(admin).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateResetTokenAsync(string email)
        {
            var admin = await _db.Admins
                .FirstOrDefaultAsync(a => a.Email == email && !a.IsDeleted);
            if (admin == null)
                return null;

            var token = Guid.NewGuid().ToString();
            admin.ResetToken = token;
            admin.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _db.SaveChangesAsync();
            return token;
        } 

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var admin = await _db.Admins
                .FirstOrDefaultAsync(a =>
                    a.Email == email &&
                    a.ResetToken == token &&
                    a.ResetTokenExpiry > DateTime.UtcNow);

            if (admin == null)
                return false;

            admin.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            admin.ResetToken = null;
            admin.ResetTokenExpiry = null;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
