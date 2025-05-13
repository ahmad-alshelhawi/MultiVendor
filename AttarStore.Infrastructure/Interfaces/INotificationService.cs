using AttarStore.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Interfaces
{
    public interface INotificationService
    {
        Task SendToUserAsync(int userId, string title, string message);
        Task SendToAdminAsync(int adminId, string title, string message);
        Task SendToClientAsync(int clientId, string title, string message);
        Task SendToVendorStoreAsync(int vendorId, string title, string message);
        Task SendToUserRoleAsync(string roleName, string title, string message);
        Task BroadcastAsync(string title, string message);
        Task<IEnumerable<NotificationDto>> GetInboxAsync(int currentUserId);
        Task<IEnumerable<NotificationDto>> GetAllAsync();
        Task MarkAsReadAsync(int userId, int notificationId);
    }
}
