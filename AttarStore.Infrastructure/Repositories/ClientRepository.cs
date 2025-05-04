using System;
using System.Linq;
using System.Threading.Tasks;
using AttarStore.Domain.Entities;
using AttarStore.Services.Data;
using AttarStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using AttarStore.Domain.Entities.submodels;

namespace AttarStore.Services.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _db;

        public ClientRepository(AppDbContext dbContext)
            => _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<bool> ExistsByNameOrEmailAsync(string name, string email, int? excludeClientId = null)
        {
            // Check across Users
            var userExists = await _db.Users
                .AnyAsync(u => !u.IsDeleted
                              && (u.Name == name || u.Email == email)
                              && (!excludeClientId.HasValue || u.Id != excludeClientId.Value));

            // Check across Admins
            var adminExists = await _db.Admins
                .AnyAsync(a => !a.IsDeleted
                              && (a.Name == name || a.Email == email)
                              && (!excludeClientId.HasValue || a.Id != excludeClientId.Value));

            // Check across Clients
            var clientExists = await _db.Clients
                .AnyAsync(c => (c.Name == name || c.Email == email)
                              && (!excludeClientId.HasValue || c.Id != excludeClientId.Value));

            // Check across Vendors (for contact email or name)
            var vendorExists = await _db.Vendors
                .AnyAsync(v => v.Name == name || v.ContactEmail == email);

            return userExists || adminExists || clientExists || vendorExists;
        }

        public async Task<IUser> GetByUsernameOrEmailAsync(string input)
        {
            var client = await _db.Clients
                .SingleOrDefaultAsync(c => c.Name == input || c.Email == input);
            if (client != null) return client;

            var user = await _db.Users
                .Where(u => !u.IsDeleted)
                .SingleOrDefaultAsync(u => u.Name == input || u.Email == input);
            if (user != null) return user;

            var admin = await _db.Admins
                .Where(a => !a.IsDeleted)
                .SingleOrDefaultAsync(a => a.Name == input || a.Email == input);
            return admin;
        }

        public async Task<Client> GetByNameAsync(string name)
            => await _db.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name == name);

        public async Task<Client> GetByEmailAsync(string email)
            => await _db.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == email);

        public async Task<Client> GetByIdAsync(int id)
            => await _db.Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Client[]> GetAllAsync()
            => await _db.Clients
                .AsNoTracking()
                .ToArrayAsync();

        public async Task<string> GetRoleByUsernameAsync(string username)
            => (await _db.Clients
                .Where(c => c.Name == username)
                .Select(c => c.Role)
                .FirstOrDefaultAsync())?.ToUpper();

        public async Task<Client> GetByRefreshTokenAsync(string refreshToken)
            => await _db.Clients
                .AsNoTracking()
                .Include(c => c.RefreshTokens)
                .FirstOrDefaultAsync(c => c.RefreshTokens
                    .Any(rt => rt.Token == refreshToken && !rt.IsRevoked));

        public async Task AddAsync(Client client)
        {
            if (client is null)
                throw new ArgumentNullException(nameof(client));

            // Uniqueness check
            if (await ExistsByNameOrEmailAsync(client.Name, client.Email))
                throw new InvalidOperationException("Name or email already exists.");

            _db.Clients.Add(client);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Client client)
        {
            if (client is null)
                throw new ArgumentNullException(nameof(client));

            // Uniqueness check excluding self
            if (await ExistsByNameOrEmailAsync(client.Name, client.Email, client.Id))
                throw new InvalidOperationException("Name or email already exists.");

            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var client = await _db.Clients.FindAsync(id);
            if (client != null)
            {
                client.IsDeleted = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
            => await ExistsByNameOrEmailAsync(null, email);

        public async Task<string> UpdateProfileAsync(int clientId, string name, string phone, string email)
        {
            var client = await GetByIdAsync(clientId);
            if (client == null) return "Client not found";

            bool updated = false;

            // Name
            if (!string.IsNullOrWhiteSpace(name) && !name.Equals(client.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (await ExistsByNameOrEmailAsync(name, client.Email, clientId))
                    return "Name already exists.";

                client.Name = name;
                updated = true;
            }

            // Email
            if (!string.IsNullOrWhiteSpace(email) && !email.Equals(client.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await ExistsByNameOrEmailAsync(client.Name, email, clientId))
                    return "Email already exists.";

                client.Email = email;
                updated = true;
            }

            // Phone
            if (!string.IsNullOrWhiteSpace(phone) && !phone.Equals(client.Phone, StringComparison.OrdinalIgnoreCase))
            {
                client.Phone = phone;
                updated = true;
            }

            if (!updated)
                return "No changes have been made.";

            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return "Profile updated successfully.";
        }

        public async Task<bool> UpdatePasswordAsync(int clientId, string currentPassword, string newPassword)
        {
            var client = await GetByIdAsync(clientId);
            if (client == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, client.Password))
                return false;

            client.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateResetTokenAsync(string email)
        {
            var client = await _db.Clients.FirstOrDefaultAsync(c => c.Email == email && !c.IsDeleted);
            if (client == null) return null;

            var token = Guid.NewGuid().ToString();
            client.ResetToken = token;
            client.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _db.SaveChangesAsync();
            return token;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var client = await _db.Clients
                .FirstOrDefaultAsync(c =>
                    c.Email == email &&
                    c.ResetToken == token &&
                    c.ResetTokenExpiry > DateTime.UtcNow);

            if (client == null)
                return false;

            client.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            client.ResetToken = null;
            client.ResetTokenExpiry = null;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
