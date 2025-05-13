using AttarStore.Domain.Entities;
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

        public async Task<Notification> AddAsync(Notification n)
        {
            var ent = await _ctx.Notifications.AddAsync(n);
            await _ctx.SaveChangesAsync();
            return ent.Entity;
        }

        public async Task AddUserLinkAsync(int nid, int uid)
        {
            _ctx.UserNotifications.Add(new UserNotification { NotificationId = nid, UserId = uid });
            await _ctx.SaveChangesAsync();
        }

        public async Task AddAdminLinkAsync(int nid, int aid)
        {
            _ctx.AdminNotifications.Add(new AdminNotification { NotificationId = nid, AdminId = aid });
            await _ctx.SaveChangesAsync();
        }

        public async Task AddClientLinkAsync(int nid, int cid)
        {
            _ctx.ClientNotifications.Add(new ClientNotification { NotificationId = nid, ClientId = cid });
            await _ctx.SaveChangesAsync();
        }

        public async Task AddVendorLinkAsync(int nid, int vid)
        {
            _ctx.VendorNotifications.Add(new VendorNotification { NotificationId = nid, VendorId = vid });
            await _ctx.SaveChangesAsync();
        }

        public Task<IEnumerable<Notification>> GetForUserAsync(int uid)
            => Task.FromResult(_ctx.UserNotifications
                 .Where(x => x.UserId == uid)
                 .Select(x => x.Notification)
                 .OrderByDescending(n => n.SentAt)
                 .AsEnumerable());

        public Task<IEnumerable<Notification>> GetAllAsync()
            => Task.FromResult(_ctx.Notifications
                 .OrderByDescending(n => n.SentAt)
                 .AsEnumerable());

        public async Task MarkAsReadAsync(int userId, int notificationId)
        {
            var link = await _ctx.UserNotifications
                .FirstOrDefaultAsync(x => x.UserId == userId && x.NotificationId == notificationId);
            if (link != null)
            {
                link.IsRead = true;
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
