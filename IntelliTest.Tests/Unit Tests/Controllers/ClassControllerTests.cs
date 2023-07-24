using IntelliTest.Controllers;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models;
using IntelliTest.Data.Enums;
using IntelliTest.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using IntelliTest.Core.Models.Classes;

namespace IntelliTest.Tests.Unit_Tests.Controllers
{
    [TestFixture]
    public class ClassControllerTests
    {
        private ClassesController classesController;
        Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        Guid id2 = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e88");

        private void SetUserRole(string roleName)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, roleName),
            }, "mock"));
            classesController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            classesController = new ClassesController(ClassServiceMock.Instance, new MemoryCache(new MemoryCacheOptions()), null, TestResultsServiceMock.Instance)
            {
                TempData = tempData
            };
            SetUserRole("Student");
        }

        [Test]
        public async Task Index_Correct()
        {
            var res = await classesController.Index();
            Assert.NotNull(res);
        }
        [Test]
        public async Task Create_Get_Student_Correct()
        {
            var res = await classesController.Create();
            Assert.NotNull(res);
        }
        [Test]
        public async Task Create_Get_Teacher_Correct()
        {
            SetUserRole("Teacher");
            var res = await classesController.Create();
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Create_Post_Student_Correct()
        {
            var res = await classesController.Create(new ClassViewModel());
            Assert.NotNull(res);
        }
        [Test]
        public async Task Create_Post_Teacher_Correct()
        {
            SetUserRole("Teacher");
            var res = await classesController.Create(new ClassViewModel());
            Assert.NotNull(res);
            SetUserRole("Student");
        }

        [Test]
        public async Task Edit_Get_Student_Correct()
        {
            var res = await classesController.Edit(id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Edit_Post_Student_Correct()
        {
            var res = await classesController.Edit(new ClassViewModel(), id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Edit_Get_Teacher_Correct()
        {
            SetUserRole("Teacher");
            var res = await classesController.Edit(id);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_Post_Teacher_FakeId_Correct()
        {
            SetUserRole("Teacher");
            var res = await classesController.Edit(new ClassViewModel(), id2);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_Post_Teacher_ValidId_Correct()
        {
            SetUserRole("Teacher");
            var res = await classesController.Edit(new ClassViewModel(), id);
            Assert.NotNull(res);
            SetUserRole("Student");
        }

        [Test]
        public async Task Delete_Post_Student_Correct()
        {
            var res = await classesController.Delete(id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Delete_Post_Teacher_Correct()
        {
            SetUserRole("Teacher");
            var res = await classesController.Delete(id);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Details_Get_Correct()
        {
            var res = await classesController.Details(id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task RemoveStudent_Post_Student_Correct()
        {
            var res = await classesController.RemoveStudent(id, id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task RemoveStudent_Post_Teacher_Correct()
        {
            SetUserRole("Teacher");
            var res = await classesController.RemoveStudent(id, id);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task AddStudent_Post_Student_Correct()
        {
            var res = await classesController.AddStudent(id, id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task AddStudent_Post_Teacher_Correct()
        {
            SetUserRole("Teacher");
            var res = await classesController.AddStudent(id, id);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public void Join_Get_Student_Correct()
        {
            var res = classesController.Join();
            Assert.NotNull(res);
        }
        [Test]
        public void Join_Get_Teacher_Correct()
        {
            SetUserRole("Teacher");
            var res = classesController.Join();
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public void Join_Post_Student_NoId()
        {
            var res = classesController.Join(new JoinModel());
            Assert.NotNull(res);
        }
        [Test]
        public void Join_Post_Teacher_Correct()
        {
            SetUserRole("Teacher");
            var res = classesController.Join(new JoinModel());
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public void Join_Post_Student_WithId()
        {
            classesController.TempData["StudentId"] = id;
            var res = classesController.Join(new JoinModel());
            Assert.NotNull(res);
        }
    }
}
