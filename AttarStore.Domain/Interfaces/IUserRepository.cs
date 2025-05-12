using AttarStore.Domain.Entities;
using AttarStore.Domain.Entities.submodels;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces
{
    public interface IUserRepository
    {
        // Existence check (excluding specified IDs when updating)
        Task<bool> ExistsByNameOrEmailAsync(
            string name,
            string email,
            int? excludeUserId = null,
            int? excludeAdminId = null,
            int? excludeClientId = null,
            int? excludeVendorId = null);

        // Lookup across Users, Admins, Clients
        Task<IUser> GetByUsernameOrEmailAsync(string input);

        // CRUD lookups
        Task<User> GetByNameAsync(string name);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(int id);
        Task<User[]> GetAllAsync();

        // Role lookup
        Task<string> GetRoleByUsernameAsync(string username);

        // Refresh–token lookup
        Task<User> GetByRefreshTokenAsync(string refreshToken);

        // Create / Update / Delete
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);

        // Uniqueness by email
        Task<bool> EmailExistsAsync(string email);

        // Profile & password operations
        Task<string> UpdateProfileAsync(
            int userId,
            string name,
            string phone,
            string email);

        Task<bool> UpdatePasswordAsync(
            int userId,
            string currentPassword,
            string newPassword);

        // Password reset flow
        Task<string> GenerateResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

        /// <summary>
        /// List all users belonging to the given vendor.
        /// </summary>
        Task<User[]> GetByVendorIdAsync(int vendorId);
        Task<User[]> GetByAdminIdAsync(int adminId);



        // → Returns all users in a given role
        Task<IEnumerable<User>> GetByRoleAsync(string roleName);

        // → Returns every user
    }
}
