using AttarStore.Domain.Entities.Auth;
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
        Task<IEnumerable<Notification>> GetByUserAsync(int userId);
        Task<Notification?> GetByIdAsync(int id);
        Task UpdateAsync(Notification notification);
        Task DeleteAsync(Notification notification);
    }
}
