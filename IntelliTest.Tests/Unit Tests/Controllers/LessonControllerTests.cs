using IntelliTest.Controllers;
using IntelliTest.Data.Enums;
using IntelliTest.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Lessons;

namespace IntelliTest.Tests.Unit_Tests.Controllers
{
    [TestFixture]
    public class LessonControllerTests
    {
        private LessonsController lessonsController;
        Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        Guid id2 = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e88");
        EditLessonViewModel editLessonModel = new EditLessonViewModel()
        {
            Title = "Title",
            Content = "No Content",
            HtmlContent = "No Content",
            School = "PPMG",
            Subject = Subject.Няма,
            Grade = 9
        };

        private void SetUserRole(string roleName)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, roleName),
            }, "mock"));
            lessonsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            lessonsController = new LessonsController(LessonServiceMock.Instance, new MemoryCache(new MemoryCacheOptions()))
            {
                TempData = tempData
            };
            SetUserRole("Student");
        }

        [Test]
        public async Task Index_Correct()
        {
            var res = await lessonsController.Index("", 0, Subject.Няма, Sorting.Likes, 0);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Read_NoId_Correct()
        {
            var res = await lessonsController.Read(id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Read_WithIdExisting_Correct()
        {
            lessonsController.TempData["TeacherId"] = id;
            var res = await lessonsController.Read(id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Read_WithFakeId_Correct()
        {
            lessonsController.TempData["TeacherId"] = id2;
            var res = await lessonsController.Read(id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Create_Student_Correct()
        {
            var res = await lessonsController.Create();
            Assert.NotNull(res);
        }
        [Test]
        public async Task Create_Teacher_Correct()
        {
            SetUserRole("Teacher");
            var res = await lessonsController.Create();
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_Student_Correct()
        {
            var res = await lessonsController.Edit(id, null);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Edit_Teacher_NoModel_NoId_Correct()
        {
            SetUserRole("Teacher");
            var res = await lessonsController.Edit(id, null);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_Teacher_NoModel_WithFakeId_Correct()
        {
            SetUserRole("Teacher");
            lessonsController.TempData["TeacherId"] = id2;
            var res = await lessonsController.Edit(id, null);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_Teacher_NoModel_WithValidId_Correct()
        {
            SetUserRole("Teacher");
            lessonsController.TempData["TeacherId"] = id;
            var res = await lessonsController.Edit(id, null);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_Teacher_WithModel_Correct()
        {
            SetUserRole("Teacher");
            var res = await lessonsController.Edit(id, null);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task SubmitEdit_Student_Correct()
        {
            var res = await lessonsController.SubmitEdit(id, editLessonModel);
            Assert.NotNull(res);
        }
        [Test]
        public async Task SubmitEdit_Teacher_NoId_Student_Correct()
        {
            SetUserRole("Teacher");
            var res = await lessonsController.SubmitEdit(id, editLessonModel);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task SubmitEdit_Teacher_WithId_Student_Correct()
        {
            SetUserRole("Teacher");
            lessonsController.TempData["TeacherId"] = id;
            var res = await lessonsController.SubmitEdit(id, editLessonModel);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Like_NoId_Correct()
        {
            var res = await lessonsController.Like(id, "StudentUser");
            Assert.NotNull(res);
        }
        [Test]
        public async Task Like_WithId_Correct()
        {
            lessonsController.TempData["StudentId"] = id;
            var res = await lessonsController.Like(id, "StudentUser");
            Assert.NotNull(res);
        }
        [Test]
        public async Task Unlike_NoId_Correct()
        {
            var res = await lessonsController.Unlike(id, "StudentUser");
            Assert.NotNull(res);
        }
        [Test]
        public async Task Unlike_WithId_Correct()
        {
            lessonsController.TempData["StudentId"] = id;
            var res = await lessonsController.Unlike(id, "StudentUser");
            Assert.NotNull(res);
        }
    }
}
