

using AttarStore.Domain.Entities;

namespace AttarStore.Domain.Interfaces
{
    public interface IAdminRepository
    {
        // Lookup
        Task<Admin> GetByUsernameAsync(string username);
        Task<Admin> GetByIdAsync(int id);
        Task<Admin> GetByEmailAsync(string email);
        Task<Admin[]> GetAllAsync();

        // Existence checks
        Task<bool> EmailExistsAsync(string email);

        // CRUD
        Task AddAsync(Admin admin);
        Task UpdateAsync(Admin admin);
        Task DeleteAsync(int id);

        // Profile & password
        Task<string> UpdateProfileAsync(int adminId, string name, string phone, string email);
        Task<bool> UpdatePasswordAsync(int adminId, string currentPassword, string newPassword);

        // Reset flow
        Task<string> GenerateResetTokenAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

    }
}
