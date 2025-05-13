using AttarStore.Application.Dtos;
using AttarStore.Domain.Entities;
using AttarStore.Domain.Entities.Auth;
using AttarStore.Domain.Interfaces;
using AttarStore.Infrastructure.Hubs;
using AttarStore.Infrastructure.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttarStore.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _nr;
        private readonly IVendorRepository _vr;
        private readonly IUserRepository _ur;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IMapper _map;

        public NotificationService(
            INotificationRepository nr,
            IVendorRepository vr,
            IUserRepository ur,
            IHubContext<NotificationHub> hub,
            IMapper map)
        {
            _nr = nr;
            _vr = vr;
            _ur = ur;
            _hub = hub;
            _map = map;
        }

        public async Task SendToUserAsync(int userId, string title, string message)
        {
            var n = await _nr.AddAsync(new Notification { Title = title, Message = message });
            await _nr.AddUserLinkAsync(n.Id, userId);
            await _hub.Clients.Group($"User_{userId}")
                      .SendAsync("ReceiveNotification", _map.Map<NotificationDto>(n));
        }

        public async Task SendToAdminAsync(int adminId, string title, string message)
        {
            var n = await _nr.AddAsync(new Notification { Title = title, Message = message });
            await _nr.AddAdminLinkAsync(n.Id, adminId);
            await _hub.Clients.Group($"Admin_{adminId}")
                      .SendAsync("ReceiveNotification", _map.Map<NotificationDto>(n));
        }

        public async Task SendToClientAsync(int clientId, string title, string message)
        {
            var n = await _nr.AddAsync(new Notification { Title = title, Message = message });
            await _nr.AddClientLinkAsync(n.Id, clientId);
            await _hub.Clients.Group($"Client_{clientId}")
                      .SendAsync("ReceiveNotification", _map.Map<NotificationDto>(n));
        }

        public async Task SendToVendorStoreAsync(int vendorId, string title, string message)
        {
            var n = await _nr.AddAsync(new Notification { Title = title, Message = message });
            await _nr.AddVendorLinkAsync(n.Id, vendorId);

            var users = await _vr.GetAssignedUsersAsync(vendorId);
            foreach (var u in users)
            {
                await _nr.AddUserLinkAsync(n.Id, u.Id);
                await _hub.Clients.Group($"User_{u.Id}")
                          .SendAsync("ReceiveNotification", _map.Map<NotificationDto>(n));
            }
        }

        public async Task SendToUserRoleAsync(string roleName, string title, string message)
        {
            var n = await _nr.AddAsync(new Notification { Title = title, Message = message });
            var users = await _ur.GetByRoleAsync(roleName);
            foreach (var u in users)
            {
                await _nr.AddUserLinkAsync(n.Id, u.Id);
                await _hub.Clients.Group($"User_{u.Id}")
                          .SendAsync("ReceiveNotification", _map.Map<NotificationDto>(n));
            }
        }

        public async Task BroadcastAsync(string title, string message)
        {
            var n = await _nr.AddAsync(new Notification { Title = title, Message = message });
            await _hub.Clients.All
                      .SendAsync("ReceiveNotification", _map.Map<NotificationDto>(n));
        }

        public async Task<IEnumerable<NotificationDto>> GetInboxAsync(int currentUserId)
        {
            var list = await _nr.GetForUserAsync(currentUserId);
            return list.Select(n => _map.Map<NotificationDto>(n));
        }

        public async Task<IEnumerable<NotificationDto>> GetAllAsync()
        {
            var all = await _nr.GetAllAsync();
            return all.Select(n => _map.Map<NotificationDto>(n));
        }

        public async Task MarkAsReadAsync(int userId, int notificationId)
            => await _nr.MarkAsReadAsync(userId, notificationId);
    }
}