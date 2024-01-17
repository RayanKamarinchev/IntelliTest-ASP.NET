using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Lessons;
using IntelliTest.Core.Models;
using IntelliTest.Data.Enums;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IntelliTest.Areas.Admin.Controllers
{
    public class LessonsController : AdminController
    {
        private readonly ILessonService lessonService;
        private readonly IMemoryCache cache;

        public LessonsController(ILessonService _lessonService, IMemoryCache _cache)
        {
            lessonService = _lessonService;
            cache = _cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string SearchTerm, int Grade, Subject Subject, Sorting Sorting, int currentPage)
        {
            if (cache.TryGetValue("adminLessons", out QueryModel<LessonViewModel>? model) && false)
            {
            }
            else
            {
                if (currentPage == 0)
                {
                    currentPage = 1;
                }
                QueryModel<LessonViewModel> query = new QueryModel<LessonViewModel>(SearchTerm, Grade, Subject, Sorting, currentPage);
                model = await lessonService.GetAllAdmin(query);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                cache.SetAsync("adminLessons", model, cacheEntryOptions);
            }
            return View(model);
        }
    }
}
