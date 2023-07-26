namespace IntelliTest.Core.Models.Chat
{
    public class ChatsViewModel
    {
        public ProfileViewModel Profile { get; set; }
        public IEnumerable<RoomViewModel> Rooms { get; set; }
    }
}
