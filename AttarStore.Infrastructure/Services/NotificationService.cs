using AttarStore.Application.Dtos;
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
        private readonly INotificationRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IMapper _mapper;

        public NotificationService(
            INotificationRepository repo,
            IUserRepository userRepo,
            IHubContext<NotificationHub> hub,
            IMapper mapper)
        {
            _repo = repo;
            _userRepo = userRepo;
            _hub = hub;
            _mapper = mapper;
        }

        public async Task<NotificationDto> CreateForUserAsync(CreateNotificationDto dto)
        {
            var entity = new Notification
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Message = dto.Message
            };
            var saved = await _repo.AddAsync(entity);
            var result = _mapper.Map<NotificationDto>(saved);

            // real-time push
            await _hub.Clients.User(dto.UserId.ToString())
                      .SendAsync("ReceiveNotification", result);

            return result;
        }

        public async Task<IEnumerable<NotificationDto>> CreateForRoleAsync(CreateNotificationDto dto, string roleName)
        {
            var users = await _userRepo.GetByRoleAsync(roleName);
            var created = new List<NotificationDto>();

            foreach (var u in users)
            {
                var entity = new Notification
                {
                    UserId = u.Id,
                    TargetRole = roleName,
                    Title = dto.Title,
                    Message = dto.Message
                };
                var saved = await _repo.AddAsync(entity);
                var dtoOut = _mapper.Map<NotificationDto>(saved);
                created.Add(dtoOut);
            }

            // group push
            await _hub.Clients.Group(roleName)
                      .SendAsync("ReceiveNotification", created);

            return created;
        }

        public async Task<IEnumerable<NotificationDto>> CreateForAllAsync(CreateNotificationDto dto)
        {
            var users = await _userRepo.GetAllAsync();
            var created = new List<NotificationDto>();

            foreach (var u in users)
            {
                var entity = new Notification
                {
                    UserId = u.Id,
                    Title = dto.Title,
                    Message = dto.Message
                };
                var saved = await _repo.AddAsync(entity);
                created.Add(_mapper.Map<NotificationDto>(saved));
            }

            await _hub.Clients.All.SendAsync("ReceiveNotification", created);
            return created;
        }

        public async Task DeleteAsync(int notificationId)
        {
            var n = await _repo.GetByIdAsync(notificationId)
                ?? throw new KeyNotFoundException("Notification not found");
            await _repo.DeleteAsync(n);
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            var list = await _repo.GetByUserAsync(userId);
            return _mapper.Map<IEnumerable<NotificationDto>>(list);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var n = await _repo.GetByIdAsync(notificationId)
                ?? throw new KeyNotFoundException("Notification not found");
            n.IsRead = true;
            await _repo.UpdateAsync(n);
        }
    }
}