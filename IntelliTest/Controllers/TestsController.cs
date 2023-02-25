using System.Security.Claims;
using IntelliTest.Contracts;
using IntelliTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IntelliTestWeb.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly ITestService testService;

        public TestsController(ITestService _testService)
        {
            testService = _testService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await testService.GetAll();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyTests()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await testService.GetMy(userId);
            return View("Index", model);
        }
    }
}
