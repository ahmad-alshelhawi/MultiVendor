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
        Task<NotificationDto> CreateForUserAsync(CreateNotificationDto dto);
        Task<IEnumerable<NotificationDto>> CreateForRoleAsync(CreateNotificationDto dto, string roleName);
        Task<IEnumerable<NotificationDto>> CreateForAllAsync(CreateNotificationDto dto);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
        Task DeleteAsync(int notificationId);
    }
}
