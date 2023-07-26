using IntelliTest.Core.Models.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models;

namespace IntelliTest.Core.Contracts
{
    public interface IRoomService
    {
        public Task<IEnumerable<RoomViewModel>> GetAll(string userId);
        public Task<RoomViewModel?> GetById(Guid id, string userId);
        public Task<RoomViewModel?> Create(RoomViewModel viewModel, string userId);
        public Task<HttpError> Edit(Guid id, RoomViewModel viewModel, string userId);
        public Task<bool> Delete(Guid id, string userId);
        public Task<bool> AddUser(Guid roomId, string userId);
        public Task<ChatsViewModel> GetChatsViewModel(string userId);
    }
}
