using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Classes;
using IntelliTest.Core.Models.Users;
using IntelliTest.Core.Services;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;
using Newtonsoft.Json;
using NUnit.Framework;
using IntelliTest.Core.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace IntelliTest.Tests.Unit_Tests
{
    [TestFixture]
    public class RoomServiceTests : UnitTestBase
    {
        private IRoomService roomService;
        private Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        private Guid id2 = Guid.Parse("fcda0a94-d7f6-4836-a093-f69066f177c7");

        [OneTimeSetUp]
        public void SetUp() =>
            roomService = new RoomService(data, null);

        [Test]
        public async Task GetAll_Correct()
        {
            var firstRoom = (await roomService.GetAll("StudentUser")).FirstOrDefault();
            Assert.AreEqual(id, firstRoom.Id);
        }

        [Test]
        public async Task GetById_Correct()
        {
            var expected = new RoomViewModel()
            {
                Admin = "Antonio Vivaldi",
                Id = id,
                Name = "Class room",
                LastMessage = "",
                TimeStamp = ""
            };
            var received = await roomService.GetById(id, "TeacherUser");
            Assert.AreEqual(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(received));
        }

        [Test]
        public async Task Create_Correct()
        {
            int oldRoomsCount = data.Rooms.Count();
            await roomService.Create(new RoomViewModel()
            {
                Name = "NewRoom"
            }, "TeacherUser");
            Assert.AreEqual(oldRoomsCount + 1, data.Rooms.Count());
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task Edit_Correct()
        {
            var room = new RoomViewModel()
            {
                Name = "Other name",
            };
            await roomService.Edit(id, room, "TeacherUser");
            var roomDb = await data.Rooms.FirstOrDefaultAsync(r=>r.Id == id);
            Assert.AreEqual("Other name", roomDb.Name);
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task Delete_Correct()
        {
            int oldRoomsCount = data.Rooms
                                    .Count(r => !r.IsDeleted);
            await roomService.Delete(id, "TeacherUser");
            Assert.AreEqual(oldRoomsCount - 1, data.Rooms.Count(r => !r.IsDeleted));
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task AddUser_Correct()
        {
            int oldRoomUsersCount = data.Rooms.FirstOrDefault().Users.Count;
            await roomService.AddUser(id, "BestUser");
            Assert.AreEqual(oldRoomUsersCount + 1, data.Rooms.FirstOrDefault().Users.Count);
            SetUpBase();
            SetUp();
        }
    }
}
