using System.Security.Claims;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Tests;
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
        public async Task<IActionResult> Edit(int id)
        {
            if (!testService.ExistsbyId(id))
            {
                return BadRequest();
            }

            var model = await testService.GetById(id);
            var testEdit = testService.ToEdit(model);
            return View(testEdit);
        }
        [HttpPost]
        public IActionResult Edit(int id, TestEditViewModel model)
        {
            if (!testService.ExistsbyId(id))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return View(model);
        }

        public IActionResult AddQuestion()
        {
            OpenQuestionViewModel model = new OpenQuestionViewModel();
            return PartialView("OpenQuestionPartialView", model);
        }
    }
}
