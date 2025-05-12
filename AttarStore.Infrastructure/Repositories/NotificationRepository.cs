using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AttarStore.Infrastructure.Services;
using AttarStore.Services.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _ctx;
        public NotificationRepository(AppDbContext ctx) => _ctx = ctx;

        public async Task<Notification> AddAsync(Notification notification)
        {
            _ctx.Notifications.Add(notification);
            await _ctx.SaveChangesAsync();
            return notification;
        }

        public async Task DeleteAsync(Notification notification)
        {
            _ctx.Notifications.Remove(notification);
            await _ctx.SaveChangesAsync();
        }

        public async Task<Notification?> GetByIdAsync(int id) =>
            await _ctx.Notifications.FindAsync(id);

        public async Task<IEnumerable<Notification>> GetByUserAsync(int userId) =>
            await _ctx.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SentAt)
                .ToListAsync();

        public async Task UpdateAsync(Notification notification)
        {
            _ctx.Notifications.Update(notification);
            await _ctx.SaveChangesAsync();
        }
    }
}
