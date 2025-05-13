
using AttarStore.Domain.Entities;
using AttarStore.Domain.Entities.submodels;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces
{
    public interface IClientRepository
    {
        // Existence check (excluding this client when updating)
        Task<bool> ExistsByNameOrEmailAsync(string name, string email, int? excludeClientId = null);

        // Lookup by username/email across Clients, Users, and Admins
        Task<IUser> GetByUsernameOrEmailAsync(string input);

        // CRUD lookups
        Task<Client> GetByNameAsync(string name);
        Task<Client> GetByEmailAsync(string email);
        Task<Client> GetByIdAsync(int id);
        Task<Client[]> GetAllAsync();

        // Role lookup
        Task<string> GetRoleByUsernameAsync(string username);

        // Refresh–token lookup
        Task<Client> GetByRefreshTokenAsync(string refreshToken);

        // Create / Update / Delete
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(int id);

        // Uniqueness by email
        Task<bool> EmailExistsAsync(string email);

        // Profile & password operations
        Task<string> UpdateProfileAsync(int clientId, string name, string phone, string email);
        Task<bool> UpdatePasswordAsync(int clientId, string currentPassword, string newPassword);

        // Password reset flow
        Task<string> GenerateResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

    }
}
