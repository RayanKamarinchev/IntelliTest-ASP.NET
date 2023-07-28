using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Services;
using NUnit.Framework;

namespace IntelliTest.Tests.Unit_Tests
{
    [TestFixture]
    public class TeacherServiceTests : UnitTestBase
    {
        private ITeacherService teacherService;
        private Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        private Guid id2 = Guid.Parse("fcda0a94-d7f6-4836-a093-f69066f177c7");

        [OneTimeSetUp]
        public void SetUp()
        {
            teacherService = new TeacherService(data);
        }

        [Test]
        public async Task AddTeacher_Correct()
        {
            string userId = "BestUser";
            string school = "BestSchool";
            await teacherService.AddTeacher(userId, school);
            Assert.IsTrue(data.Teachers.Any(t=>t.UserId == userId && t.School == school));
        }

        [Test]
        public async Task GetTeacherId_Correct()
        {
            string userId = "TeacherUser";
            Guid? actualId = teacherService.GetTeacherId(userId);
            Assert.AreEqual(id, actualId);
        }
    }
}
