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
        private readonly IHubContext<ChatHub> _hubContext;

        public RoomService(IntelliTestDbContext _context,
                           IHubContext<ChatHub> hubContext)
        {
            context = _context;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<RoomViewModel>> GetAll()
        {
            var rooms = await context.Rooms
                                     .Include(r => r.Admin)
                                     .Include(r => r.Messages)
                                     .Where(r => !r.IsDeleted)
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
        
        public async Task<RoomViewModel?> GetById(Guid id)
        {
            var room = await context.Rooms.Where(r=> !r.IsDeleted).FirstOrDefaultAsync(r=>r.Id==id);
            if (room == null)
                return null;
            return new RoomViewModel()
            {
                Admin = room.Admin.UserName,
                Id = room.Id,
                Name = room.Name,
                LastMessage = room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault() == null ? "" : room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault().Content,
                TimeStamp = room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault() == null ? "" : room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault().Timestamp.ToString("MM/dd/yyyy")
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
            var user = context.Users.Find(userId);
            var createdRoom = new RoomViewModel()
            {
                Admin = room.Admin.UserName,
                Id = room.Id,
                Name = room.Name,
                LastMessage = room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault() == null ? "" : room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault().Content,
                TimeStamp = room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault() == null ? "" : room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault().Timestamp.ToString("MM/dd/yyyy")
            };
            await _hubContext.Clients.All.SendAsync("addChatRoom", createdRoom);

            return createdRoom;
        }
        
        public async Task<HttpError> Edit(Guid id, RoomViewModel viewModel, string userId)
        {
            if (context.Rooms.Where(r=>!r.IsDeleted).Any(r => r.Name == viewModel.Name))
                //return BadRequest("Invalid room name or room already exists");
                return HttpError.BadRequest;

            var room = await context.Rooms
                .Include(r => r.Admin)
                .Where(r => r.Id == id && r.AdminId == userId && !r.IsDeleted)
                .FirstOrDefaultAsync();

            if (room == null)
                return HttpError.NotFound;

            room.Name = viewModel.Name;
            await context.SaveChangesAsync();

            var updatedRoom = new RoomViewModel()
            {
                Admin = room.Admin.UserName,
                Id = room.Id,
                Name = room.Name,
                LastMessage = room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault() == null ? "" : room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault().Content,
                TimeStamp = room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault() == null ? "" : room.Messages.OrderBy(m => m.Timestamp).FirstOrDefault().Timestamp.ToString("MM/dd/yyyy")
            };
            await _hubContext.Clients.All.SendAsync("updateChatRoom", updatedRoom);

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

            await _hubContext.Clients.All.SendAsync("removeChatRoom", room.Id);
            await _hubContext.Clients.Group(room.Name).SendAsync("onRoomDeleted");

            return true;
        }
    }
}
