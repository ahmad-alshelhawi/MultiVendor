using AttarStore.Domain.Entities;
using AttarStore.Domain.Entities.submodels;
using AttarStore.Domain.Interfaces;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AttarStore.Services.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext dbContext)
            => _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<bool> ExistsByNameOrEmailAsync(
            string name,
            string email,
            int? excludeUserId = null,
            int? excludeAdminId = null,
            int? excludeClientId = null,
            int? excludeVendorId = null)
        {
            // Clients
            var clientExists = await _db.Clients.AnyAsync(c =>
                (c.Name == name || c.Email == email)
                && (!excludeClientId.HasValue || c.Id != excludeClientId.Value));

            // Users
            var userExists = await _db.Users.AnyAsync(u =>
                !u.IsDeleted
                && (u.Name == name || u.Email == email)
                && (!excludeUserId.HasValue || u.Id != excludeUserId.Value));

            // Admins
            var adminExists = await _db.Admins.AnyAsync(a =>
                !a.IsDeleted
                && (a.Name == name || a.Email == email)
                && (!excludeAdminId.HasValue || a.Id != excludeAdminId.Value));

            // Vendors (contact email or name)
            var vendorExists = await _db.Vendors.AnyAsync(v =>
                (v.Name == name || v.ContactEmail == email)
                && (!excludeVendorId.HasValue || v.Id != excludeVendorId.Value));

            return clientExists || userExists || adminExists || vendorExists;
        }

        public async Task<IUser> GetByUsernameOrEmailAsync(string input)
        {
            // Try User
            var user = await _db.Users
                .Where(u => !u.IsDeleted)
                .SingleOrDefaultAsync(u => u.Name == input || u.Email == input);
            if (user != null) return user;

            // Try Admin
            var admin = await _db.Admins
                .Where(a => !a.IsDeleted)
                .SingleOrDefaultAsync(a => a.Name == input || a.Email == input);
            if (admin != null) return admin;

            // Try Client
            var client = await _db.Clients
                .SingleOrDefaultAsync(c => c.Name == input || c.Email == input);
            return client;
        }

        public async Task<User> GetByNameAsync(string name)
            => await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Name == name);

        public async Task<User> GetByEmailAsync(string email)
            => await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Email == email);

        public async Task<User> GetByIdAsync(int id)
            => await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Id == id);

        public async Task<User[]> GetAllAsync()
            => await _db.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .ToArrayAsync();

        public async Task<string> GetRoleByUsernameAsync(string username)
        {
            var role = await _db.Users
                .Where(u => !u.IsDeleted && u.Name == username)
                .Select(u => u.Role)
                .FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(role))
                return role.ToUpper();

            var adminRole = await _db.Admins
                .Where(a => !a.IsDeleted && a.Name == username)
                .Select(a => a.Role)
                .FirstOrDefaultAsync();
            return adminRole?.ToUpper() ?? "UNKNOWN";
        }

        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
            => await _db.Users
                .Include(u => u.RefreshTokens)
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.RefreshTokens.Any(rt => rt.Token == refreshToken && !rt.IsRevoked));

        public async Task AddAsync(User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (await ExistsByNameOrEmailAsync(user.Name, user.Email))
                throw new InvalidOperationException("Name or email already exists.");
            // If VendorId provided, ensure the vendor exists
            if (user.VendorId.HasValue)
            {
                bool exists = await _db.Vendors.AnyAsync(v => v.Id == user.VendorId.Value);
                if (!exists)
                    throw new InvalidOperationException($"Vendor {user.VendorId} not found.");
            }
            // If AdminId provided, ensure the admin exists
            if (user.AdminId.HasValue)
            {
                var exists = await _db.Admins.AnyAsync(a => a.Id == user.AdminId);
                if (!exists)
                    throw new InvalidOperationException($"Admin {user.AdminId} not found.");
            }
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (await ExistsByNameOrEmailAsync(
                user.Name, user.Email,
                excludeUserId: user.Id))
            {
                throw new InvalidOperationException("Name or email already exists.");
            }

            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user != null)
            {
                user.IsDeleted = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
            => await _db.Users.AnyAsync(u => !u.IsDeleted && u.Email == email)
               || await _db.Admins.AnyAsync(a => !a.IsDeleted && a.Email == email)
               || await _db.Clients.AnyAsync(c => c.Email == email);

        public async Task<string> UpdateProfileAsync(
            int userId, string name, string phone, string email)
        {
            var user = await GetByIdAsync(userId);
            if (user == null) return "User not found";

            bool updated = false;

            // Name
            if (!string.IsNullOrWhiteSpace(name)
                && !name.Equals(user.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await ExistsByNameOrEmailAsync(
                    name, user.Email, excludeUserId: userId))
                {
                    return "Name already exists.";
                }
                user.Name = name;
                updated = true;
            }

            // Email
            if (!string.IsNullOrWhiteSpace(email)
                && !email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await ExistsByNameOrEmailAsync(
                    user.Name, email, excludeUserId: userId))
                {
                    return "Email already exists.";
                }
                user.Email = email;
                updated = true;
            }

            // Phone
            if (!string.IsNullOrWhiteSpace(phone)
                && !phone.Equals(user.Phone, StringComparison.OrdinalIgnoreCase))
            {
                user.Phone = phone;
                updated = true;
            }

            if (!updated)
                return "No changes have been made.";

            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return "Profile updated successfully.";
        }

        public async Task<bool> UpdatePasswordAsync(
            int userId, string currentPassword, string newPassword)
        {
            var user = await GetByIdAsync(userId);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
                return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateResetTokenAsync(string email)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
            if (user == null) return null;

            var token = Guid.NewGuid().ToString();
            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _db.SaveChangesAsync();
            return token;
        }

        public async Task<bool> ResetPasswordAsync(
            string email, string token, string newPassword)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u =>
                    u.Email == email
                    && u.ResetToken == token
                    && u.ResetTokenExpiry > DateTime.UtcNow);

            if (user == null) return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<User[]> GetByVendorIdAsync(int vendorId)
            => await _db.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted && u.VendorId == vendorId)
                .ToArrayAsync();


        public async Task<User[]> GetByAdminIdAsync(int adminId)
    => await _db.Users
        .AsNoTracking()
        .Where(u => !u.IsDeleted && u.AdminId == adminId)
        .ToArrayAsync();

        public async Task<IEnumerable<User>> GetByRoleAsync(string roleName)
            => await _db.Users
                         .Where(u => u.Role == roleName)
                         .ToListAsync();

    }
}
