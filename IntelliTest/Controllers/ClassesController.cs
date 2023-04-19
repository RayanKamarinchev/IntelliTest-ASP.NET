using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Classes;
using IntelliTest.Core.Models.Users;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace IntelliTest.Controllers
{
    public class ClassesController : Controller
    {
        private readonly IClassService classService;
        private readonly ITeacherService teacherService;
        private readonly IDistributedCache cache;

        public ClassesController(IClassService _classService, IDistributedCache _cache, ITeacherService _teacherService)
        {
            classService = _classService;
            cache = _cache;
            teacherService = _teacherService;
        }
        public async Task<IActionResult> Index()
        {
            if (1 == 0 && cache.TryGetValue("tests", out IEnumerable<ClassViewModel>? model))
            {
            }
            else
            {
                model = await classService.GetAll();
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                await cache.SetAsync("tests", model, cacheEntryOptions);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClassViewModel model)
        {
            if (User.IsStudent())
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Teacher = new TeacherViewModel()
            {
                Id = await teacherService.GetTeacherId(User.Id())
            };
            await classService.Create(model);
            return RedirectToAction();
        }
    }
}
