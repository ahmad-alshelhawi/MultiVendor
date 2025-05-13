using AttarStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification> AddAsync(Notification notification);

        Task AddUserLinkAsync(int notificationId, int userId);
        Task AddAdminLinkAsync(int notificationId, int adminId);
        Task AddClientLinkAsync(int notificationId, int clientId);
        Task AddVendorLinkAsync(int notificationId, int vendorId);

        Task<IEnumerable<Notification>> GetForUserAsync(int userId);
        Task<IEnumerable<Notification>> GetAllAsync();
        Task MarkAsReadAsync(int userId, int notificationId);
    }

}
