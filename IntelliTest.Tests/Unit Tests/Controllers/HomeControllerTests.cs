using IntelliTest.Controllers;
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

namespace IntelliTest.Tests.Unit_Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController homeController;

        private void SetUserRole(string roleName)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, roleName),
            }, "mock"));
            homeController.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            homeController = new HomeController()
            {
                TempData = tempData
            };
            SetUserRole("Student");
        }

        [Test]
        public void Index_Correct()
        {
            var res = homeController.Index();
            Assert.NotNull(res);
        }
        [Test]
        public void ErrorMessage500_Correct()
        {
            var res = homeController.Error(500);
            Assert.NotNull(res);
        }
        [Test]
        public void ErrorMessage404_Correct()
        {
            var res = homeController.Error(404);
            Assert.NotNull(res);
        }
        [Test]
        public void ErrorMessage401_Correct()
        {
            var res = homeController.Error(401);
            Assert.NotNull(res);
        }
        [Test]
        public void ErrorMessageOther_Correct()
        {
            var res = homeController.Error(403);
            Assert.NotNull(res);
        }
    }
}
