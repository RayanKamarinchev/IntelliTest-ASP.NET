using System.Security.Claims;
using IntelliTest.Controllers;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Questions.Closed;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models.Tests.Groups;
using IntelliTest.Data.Enums;
using IntelliTest.Tests.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using static IntelliTest.Infrastructure.Constraints;

namespace IntelliTest.Tests.Unit_Tests.Controllers
{
    [TestFixture]
    public class TestsControllerTests
    {
        private TestsController testsController;
        Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        Guid id2 = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e88");
        TestGroupEditViewModel testModel = new TestGroupEditViewModel();

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
            testsController = new TestsController(TestServiceMock.Instance, new MemoryCache(new MemoryCacheOptions()), StudentsServiceMock.Instance, ClassServiceMock.Instance,TestResultsServiceMock.Instance, IWebHostEnvironmentMock.Instance)
            {
                TempData = tempData
            };

            //testsController.Create(new TestViewModel()
            //{
            //    Id = id2
            //});
            SetUserRole("Student");
        }

        [Test]
        public async Task Index_Correct()
        {
            var res = await testsController.Index("", 0, Subject.Няма, Sorting.Likes, 0);
            Assert.NotNull(res);
        }
        [Test]
        public void GetFilter_Correct()
        {
            var res = testsController.GetFilter(new Filter());
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
            var res = await testsController.Edit(testModel, id, id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Edit_PublicityLevelClassOnly_NoId_Get_Correct()
        {
            SetUserRole("Teacher");
            var res = await testsController.Edit(testModel, id, id);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_PublicityLevelClassOnly_WithId1_Get_Correct()
        {
            SetUserRole("Teacher");
            testsController.TempData["TeacherId"] = id;
            var res = await testsController.Edit(new TestGroupEditViewModel()
            {
                PublicityLevel = PublicityLevel.ClassOnly
            }, id, id);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_PublicityLevelClassOnly_WithId2_Get_Correct()
        {
            SetUserRole("Teacher");
            testsController.TempData["TeacherId"] = id2;
            var res = await testsController.Edit(new TestGroupEditViewModel()
            {
                PublicityLevel = PublicityLevel.ClassOnly
            }, id, id);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_Student_NoId_Post_Correct()
        {
            var res = await testsController.Edit(testModel, id, id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Edit_Teacher_Invalid_Post_Correct()
        {
            SetUserRole("Teacher");
            var res = await testsController.Edit(new TestGroupEditViewModel()
            {
                Id = id
            }, id, id);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_Teacher_Valid_Post_Correct()
        {
            SetUserRole("Teacher");
            var res = await testsController.Edit(new TestGroupEditViewModel()
            {
                Id = id,
                OpenQuestions = new List<OpenQuestionViewModel>(),
                ClosedQuestions = new List<ClosedQuestionViewModel>(),
                Description = "Test",
                Grade = 8,
                PublicityLevel = PublicityLevel.Public,
                Time = 10,
                Title = "Tests"
            }, id, id);
            Assert.NotNull(res);
            SetUserRole("Student");
        }

        [Test]
        public async Task Edit_Teacher_Get_Correct()
        {
            SetUserRole("Teacher");
            var res = await testsController.Edit(testModel, id, id);
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Edit_Teacher_Post_Correct()
        {
            SetUserRole("Teacher");
            var res = await testsController.Edit(testModel, id, id);
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
        public async Task Create_Teacher_Post_Correct()
        {
            SetUserRole("Teacher");
            testsController.TempData["TeacherId"] = id;
            testsController.TempData.Remove("Classes");
            var res = await testsController.Create(new TestViewModel());
            Assert.NotNull(res);
            SetUserRole("Student");
        }
        [Test]
        public async Task Take_Get_WithId_Correct()
        {
            testsController.TempData["StudentId"] = id;
            var res = await testsController.Take(id2);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Take_Post_NoId_Correct()
        {
            testsController.TempData[StudentId] = id;
            testsController.TempData["GroupId"] = id;
            var res = await testsController.Take(new TestGroupSubmitViewModel(), id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Take_Post_WithId_Correct()
        {
            testsController.TempData[StudentId] = id;
            testsController.TempData["GroupId"] = id;
            var res = await testsController.Take(new TestGroupSubmitViewModel(), id);
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
        public async Task Delete_NoId_Correct()
        {
            var res = await testsController.Delete(id);
            Assert.NotNull(res);
        }
        [Test]
        public async Task Delete_WithId_Correct()
        {
            testsController.TempData["TeacherId"] = id;
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
