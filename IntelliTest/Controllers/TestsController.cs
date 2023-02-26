using System.Security.Claims;
using IntelliTest.Core.Contracts;
using IntelliTest.Infrastructure;
using IntelliTest.Models.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace IntelliTest.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly ITestService testService;
        private readonly IDistributedCache cache;

        public TestsController(ITestService _testService, IDistributedCache _cache)
        {
            testService = _testService;
            cache = _cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            cache.Remove("tests");
            if (cache.TryGetValue("tests", out IEnumerable<TestViewModel>? model))
            {
            }
            else
            {
                model = await testService.GetAll();
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromSeconds(60));
                await cache.SetAsync("tests", model, cacheEntryOptions);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyTests()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await testService.GetMy(userId);
            return View("Index", model);
        }

        [Authorize]
        public async Task<IActionResult> Details(int testId)
        {
            if (!testService.ExistsbyId(testId+1))
            {
                return BadRequest();
            }
            var model = await testService.GetById(testId+1);
            return View(model);
        }
    }
}
