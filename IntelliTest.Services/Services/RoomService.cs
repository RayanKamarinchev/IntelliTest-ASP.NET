using AutoMapper;
using Google;
using IntelliTest.Core.Hubs;
using IntelliTest.Data.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Chat;
using IntelliTest.Data;
using Microsoft.EntityFrameworkCore;

namespace IntelliTest.Core.Services
{
    public class RoomService : IRoomService
    {
        private readonly IntelliTestDbContext context;
        private readonly IHubContext<ChatHub>? _hubContext;

        public RoomService(IntelliTestDbContext _context, IHubContext<ChatHub>? _hubContext)
        {
            context = _context;
            this._hubContext = _hubContext;
        }

        private async Task<ProfileViewModel> GetCurrentUserProfile(string id)
        {
            User user = await context.Users.FindAsync(id);
            return new ProfileViewModel()
            {
                AvatarPhotoPath = user.PhotoPath,
                Name = user.FirstName + " " + user.LastName,
            };
        }
        public async Task<ChatsViewModel> GetChatsViewModel(string userId)
        {
            ProfileViewModel profile = await GetCurrentUserProfile(userId);
            return new ChatsViewModel()
            {
                Profile = profile,
                Rooms = await GetAll(userId)
            };
        }

        public async Task<IEnumerable<RoomViewModel>> GetAll(string userId)
        {
            var rooms = await context.Rooms
                                     .Include(r => r.Admin)
                                     .Include(r => r.Messages)
                                     .Where(r => !r.IsDeleted && r.Users.Any(u=>u.UserId==userId))
                                     .Select(room => new RoomViewModel()
                                     {
                                         Admin = room.Admin.UserName,
                                         Id = room.Id,
                                         Name = room.Name,
                                         LastMessage = room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault() == null ? "" : room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault().Content,
                                         TimeStamp = room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault() == null ? "" : room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault().Timestamp.ToString("MM/dd/yyyy")
                                     })
                                     .ToListAsync();
            return rooms;
        }
        
        public async Task<RoomViewModel?> GetById(Guid id, string userId)
        {
            var room = await context.Rooms
                                    .Where(r => !r.IsDeleted && r.Users.Any(u => u.UserId == userId))
                                    .FirstOrDefaultAsync(r=>r.Id==id);
            if (room == null)
                return null;
            return new RoomViewModel()
            {
                Admin = room.Admin.FirstName + " " + room.Admin.LastName,
                Id = room.Id,
                Name = room.Name,
                LastMessage = room.Messages.MinBy(m => m.Timestamp) == null ? "" : room.Messages.MinBy(m => m.Timestamp).Content,
                TimeStamp = room.Messages.MinBy(m => m.Timestamp) == null ? "" : room.Messages.MinBy(m => m.Timestamp).Timestamp.ToString("MM/dd/yyyy")
            };
        }
        
        public async Task<RoomViewModel?> Create(RoomViewModel viewModel, string userId)
        {
            if (context.Rooms.Where(r =>!r.IsDeleted).Any(r => r.Name == viewModel.Name))
                //return BadRequest("Invalid room name or room already exists");
                return null;

            var room = new Room()
            {
                Name = viewModel.Name,
                AdminId = userId
            };

            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();
            var createdRoom = new RoomViewModel()
            {
                Admin = room.Admin.FirstName + " " + room.Admin.LastName,
                Id = room.Id,
                Name = room.Name,
                LastMessage = "",
                TimeStamp = ""
            };
            if (_hubContext is not null)
            {
                await _hubContext.Clients.All.SendAsync("addChatRoom", createdRoom);
            }

            return createdRoom;
        }
        
        public async Task<HttpError> Edit(Guid id, RoomViewModel viewModel, string userId)
        {
            if (context.Rooms.Where(r=>!r.IsDeleted).Any(r => r.Name == viewModel.Name))
                return HttpError.NotFound;

            var room = await context.Rooms
                .Include(r => r.Admin)
                .Include(r => r.Messages)
                .Where(r => r.Id == id && r.AdminId == userId && !r.IsDeleted)
                .FirstOrDefaultAsync();

            if (room == null)
                return HttpError.NotFound;

            room.Name = viewModel.Name;
            await context.SaveChangesAsync();

            var updatedRoom = new RoomViewModel()
            {
                Admin = room.Admin.FirstName + " " + room.Admin.LastName,
                Id = room.Id,
                Name = room.Name,
                LastMessage = room.Messages.MinBy(m => m.Timestamp) is null? "" : room.Messages.MinBy(m => m.Timestamp).Content,
                TimeStamp = room.Messages.MinBy(m => m.Timestamp) is null ? "" : room.Messages.MinBy(m => m.Timestamp).Timestamp.ToString("MM/dd/yyyy")
            };
            if (_hubContext is not null)
            {
                await _hubContext.Clients.All.SendAsync("updateChatRoom", updatedRoom);
            }

            return HttpError.Ok;
        }
        
        public async Task<bool> Delete(Guid id, string userId)
        {
            var room = await context.Rooms
                .Include(r => r.Admin)
                .Where(r => r.Id == id && r.AdminId == userId && !r.IsDeleted)
                .FirstOrDefaultAsync();

            if (room == null)
                return false;

            room.IsDeleted = true;
            await context.SaveChangesAsync();

            if (_hubContext is not null)
            {
                await _hubContext.Clients.All.SendAsync("removeChatRoom", room.Id);
                await _hubContext.Clients.Group(room.Name).SendAsync("onRoomDeleted");
            }

            return true;
        }

        public async Task<bool> AddUser(Guid roomId, string userId)
        {
            var room = await context.Rooms
                                    .Include(r => r.Users)
                                    .Where(r => !r.IsDeleted && r.Id == roomId)
                                    .FirstOrDefaultAsync();
            if (room == null)
            {
                return false;
            }

            if (room.Users.Any(u=>u.UserId==userId))
            {
                return false;
            }
            room.Users.Add(new RoomUser()
            {
                UserId = userId
            });
            await context.SaveChangesAsync();
            return true;
        }
    }
}
