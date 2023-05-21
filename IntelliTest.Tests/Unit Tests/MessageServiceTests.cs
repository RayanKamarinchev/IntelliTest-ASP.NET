using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Chat;
using IntelliTest.Core.Services;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Tests.Unit_Tests
{
    [TestFixture]
    public class MessageServiceTests : UnitTestBase
    {
        private IMessageService messageService;
        private Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        private Guid id2 = Guid.Parse("fcda0a94-d7f6-4836-a093-f69066f177c7");

        [OneTimeSetUp]
        public void SetUp() =>
            messageService = new MessageService(data, null);

        [Test]
        public async Task GetMessages_Correct()
        {
            var firstMsg = (await messageService.GetMessages("Class room")).FirstOrDefault();
            Assert.AreEqual(id, firstMsg.Id);
        }

        [Test]
        public async Task GetById_Correct()
        {
            var expected = new MessageViewModel()
            {
                Content = "Hello",
                FromFullName = "Pesho Peshov",
                FromUserName = null,
                Id = id,
                Room = "Class room",
                Timestamp = new DateTime(2023, 5, 21, 2, 41, 0),
                Avatar = "fillUserAvatar"
            };
            var received = await messageService.GetById(id);
            Assert.AreEqual(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(received));
        }
        [Test]
        public async Task GetRoomIdByName_Correct()
        {
            Assert.AreEqual(id, await messageService.GetRoomIdByName("Class room"));
        }

        [Test]
        public async Task Create_Correct()
        {
            int oldMessagesCount = data.Messages.Count();
            await messageService.Create(new MessageViewModel()
            {
                Content = "Hello everyone",
                Room = "Class room"
            }, "TeacherUser");
            Assert.AreEqual(oldMessagesCount + 1, data.Messages.Count());
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task Delete_Correct()
        {
            int oldMessagesCount = data.Messages
                                    .Count(r => !r.IsDeleted);
            await messageService.Delete(id2, "TeacherUser");
            Assert.AreEqual(oldMessagesCount - 1, data.Messages.Count(r => !r.IsDeleted));
            SetUpBase();
            SetUp();
        }
    }
}
