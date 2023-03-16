using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Lessons;
using IntelliTest.Core.Services;
using IntelliTest.Infrastructure;
using IntelliTest.Models.Tests;
using IntelliTest.Services.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Extensions.Caching.Distributed;

namespace IntelliTest.Controllers
{
    [Authorize]
    public class LessonsController : Controller
    {
        const string SCRIPT_NAME = "script.py";
        private readonly ILessonService lessonService;
        private readonly IDistributedCache cache;
        private readonly IStudentService studentService;
        private readonly ITeacherService teacherService;

        public LessonsController(ILessonService _lessonService, IDistributedCache _cache, IStudentService _studentService, ITeacherService _teacherService)
        {
            lessonService = _lessonService;
            cache = _cache;
            studentService = _studentService;
            teacherService = _teacherService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (cache.TryGetValue("lessons", out IEnumerable<LessonViewModel>? model))
            {
            }
            else
            {
                model = await lessonService.GetAll();
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                await cache.SetAsync("lessons", model, cacheEntryOptions);
            }
            return View(model);
        }

        [HttpGet]
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (!await lessonService.ExistsById(id))
            {
                return BadRequest();
            }

            LessonViewModel model = await lessonService.GetById(id);
            return View(model);
        }

        [HttpGet]
        [Route("Lessons/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await lessonService.ExistsById(id))
            {
                return BadRequest();
            }

            if (!(bool)TempData.Peek("isTeacher"))
            {
                return Unauthorized();
            }

            int teacherId = await teacherService.GetTeacherId(User.Id());

            if (!await teacherService.IsLessonCreator(id, teacherId))
            {
                return Unauthorized();
            }

            LessonViewModel model = await lessonService.GetById(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, LessonViewModel model)
        {
            if (!await lessonService.ExistsById(id))
            {
                return BadRequest();
            }

            if (!(bool)TempData.Peek("isTeacher"))
            {
                return Unauthorized();
            }

            int teacherId = await teacherService.GetTeacherId(User.Id());

            if (!await teacherService.IsLessonCreator(id, teacherId))
            {
                return Unauthorized();
            }

            await lessonService.Edit(id, model);
            return View("Index");
        }
    }
}
