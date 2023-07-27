using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Services;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using IntelliTest.Infrastructure;

namespace IntelliTest.Areas.Admin.Controllers
{
    public class TestsController : AdminController
    {
        private readonly ITestService testService;
        private readonly IMemoryCache cache;

        public TestsController(ITestService _testService, IMemoryCache cache)
        {
            testService = _testService;
            this.cache = cache; 
        }

        [HttpGet]
        public async Task<IActionResult> Index(string SearchTerm, int Grade, Subject Subject, Sorting Sorting, int currentPage)
        {
            if (cache.TryGetValue("tests", out QueryModel<TestViewModel>? model) && false)
            {
            }
            else
            {
                if (currentPage == 0)
                {
                    currentPage = 1;
                }
                QueryModel<TestViewModel> query = new QueryModel<TestViewModel>(SearchTerm, Grade, Subject, Sorting, currentPage);
                model = await testService.GetAllAdmin(query);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                cache.SetAsync("tests", model, cacheEntryOptions);
            }
            return View(model);
        }
    }
}
