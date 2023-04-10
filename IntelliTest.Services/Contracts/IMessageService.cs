using IntelliTest.Core.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Contracts
{
    public interface IMessageService
    {
        public Task<MessageViewModel?> GetById(Guid id);
        public Task<Guid?>? GetRoomIdByName(string name);
        public Task<IEnumerable<MessageViewModel>?> GetMessages(string roomName);
        public Task<MessageViewModel?> Create(MessageViewModel viewModel, string userId);
        public Task<bool> Delete(Guid id, string userId);
    }
}
