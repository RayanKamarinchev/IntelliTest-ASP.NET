using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Hubs;
using IntelliTest.Core.Models.Chat;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace IntelliTest.Core.Services
{
    public class MessageService : IMessageService
    {
        private readonly IntelliTestDbContext context;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageService(IntelliTestDbContext _context,
                              IMapper mapper,
                              IHubContext<ChatHub> hubContext)
        {
            context = _context;
            _hubContext = hubContext;
        }

        public async Task<MessageViewModel> GetById(Guid id)
        {
            var message = await context.Messages.FindAsync(id);
            return new MessageViewModel()
            {
                Content = message.Content,
                FromFullName = message.Sender.FirstName + " " + message.Sender.LastName,
                FromUserName = message.Sender.UserName,
                Id = id,
                Room = message.Room.Name,
                Timestamp = message.Timestamp,
                Avatar = "fillUserAvatar"
            };
        }

        public async Task<Guid?>? GetRoomIdByName(string name)
        {
            var room = await context.Rooms.FirstOrDefaultAsync(r => r.Name == name);
            if (room == null) return null;
            return room.Id;
        }

        public async Task<IEnumerable<MessageViewModel>> GetMessages(string roomName)
        {
            Guid? roomId = await GetRoomIdByName(roomName);
            if (roomId==null)
            {
                return null;
            }
            var messages = await context.Messages.Where(m => m.ToRoomId == roomId)
                                   .Include(m => m.Sender)
                                   .Include(m => m.Room)
                                   .OrderByDescending(m => m.Timestamp)
                                   .Take(20)
                                   .Reverse()
                                   .Select(m=>new MessageViewModel()
                                   {
                                       Content = m.Content,
                                       FromFullName = m.Sender.FirstName + " " + m.Sender.LastName,
                                       FromUserName = m.Sender.UserName,
                                       Id = m.Id,
                                       Room = m.Room.Name,
                                       Timestamp = m.Timestamp,
                                       Avatar = "fillUserAvatar"
                                   })
                                   .ToListAsync();
            return messages;
        }
        public async Task<MessageViewModel> Create(MessageViewModel viewModel, string userId)
        {
            var room = await context.Rooms.FirstOrDefaultAsync(r => r.Name == viewModel.Room);
            if (room == null)
                return null;

            var message = new Message()
            {
                Content = Regex.Replace(viewModel.Content, @"<.*?>", string.Empty),
                SenderId = userId,
                Room = room,
                Timestamp = DateTime.Now
            };

            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();

            // Broadcast the message
            var createdMessage = new MessageViewModel()
            {
                Content = message.Content,
                FromFullName = message.Sender.FirstName + " " + message.Sender.LastName,
                FromUserName = message.Sender.UserName,
                Id = message.Id,
                Room = message.Room.Name,
                Timestamp = message.Timestamp,
                Avatar = "fillUserAvatar"
            };
            await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", createdMessage);

            return createdMessage;
        }

        public async Task<bool> Delete(Guid id, string userId)
        {
            var message = await context.Messages
                                        .Include(u => u.Sender)
                                        .Where(m => m.Id == id && m.SenderId == userId)
                                        .FirstOrDefaultAsync();

            if (message == null)
                return false;

            message.IsDeleted = true;
            await context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("removeChatMessage", message.Id);
            return true;
        }
    }
}
