using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Controllers;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Data.Enums;
using IntelliTest.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using OpenAI_API.Moderation;

namespace IntelliTest.Tests.Unit_Tests.Controllers
{
    [TestFixture]
    public class TestsControllerTests
    {
        private TestsController testsController;
        Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");

        private void SetUserRole(string roleName)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, roleName),
            }, "mock"));
            testsController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext(){User = user}
            };
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            testsController = new TestsController(TestServiceMock.Instance, new MemoryCache(new MemoryCacheOptions()), StudentsServiceMock.Instance, ClassServiceMock.Instance)
            {
                TempData = tempData
            };
            SetUserRole("Student");
        }

        [Test]
        public async Task Index_Correct()
        {
            var res = await testsController.Index("", 0, Subject.Няма, Sorting.Likes, 0);
            Assert.NotNull(res);
        }
        [Test]
        public async Task MyTests_Correct()
        {
            var res = await testsController.MyTests(new QueryModel<TestViewModel>());
            Assert.NotNull(res);
        }

        [Test]
        public async Task Edit_Student_Get_Correct()
        {
            var res = await testsController.Edit(id, new TestEditViewModel());
            Assert.NotNull(res);
        }
        [Test]
        public async Task Edit_PublicityLevelClassOnly_Get_Correct()
        {
            var res = await testsController.Edit(id, new TestEditViewModel()
            {
                PublicityLevel = PublicityLevel.ClassOnly
            });
            Assert.NotNull(res);
        }
        [Test]
        public async Task Edit_Student_Post_Correct()
        {
            var res = await testsController.Edit(new TestEditViewModel());
            Assert.NotNull(res);
        }

        [Test]
        public async Task Edit_Teacher_Get_Correct()
        {
            SetUserRole("Teacher");
            var res = await testsController.Edit(id, new TestEditViewModel());
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_Teacher_Post_Correct()
        {
            SetUserRole("Teacher");
            var res = await testsController.Edit(new TestEditViewModel());
            Assert.NotNull(res);
            SetUserRole("Student");
        }

        [Test]
        public async Task Create_Get_Correct()
        {
            var res = await testsController.Create();
            Assert.NotNull(res);
        }
        [Test]
        public async Task Create_Post_Correct()
        {
            var res = await testsController.Create(new TestViewModel());
            Assert.NotNull(res);
        }
        [Test]
        public async Task Take_Get_Correct()
        {
            var res = await testsController.Take(id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Take_Post_Correct()
        {
            var res = await testsController.Take(new TestSubmitViewModel(), id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task ReviewAnswers_Get_Correct()
        {
            var res = await testsController.ReviewAnswers(id, id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Statistics_Get_Correct()
        {
            var res = await testsController.Statistics(id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Delete_Correct()
        {
            var res = await testsController.Delete(id);
            Assert.NotNull(res);
        }
        [Test]
        public void AddQuestion_Get_Correct()
        {
            var res = testsController.AddQuestion(new OpenQuestionViewModel());
            Assert.NotNull(res);
        }
    }
}
