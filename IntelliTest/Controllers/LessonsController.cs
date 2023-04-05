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
        private readonly ITeacherService teacherService;

        public LessonsController(ILessonService _lessonService, IDistributedCache _cache, ITeacherService _teacherService)
        {
            lessonService = _lessonService;
            cache = _cache;
            teacherService = _teacherService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (0 == 1 && cache.TryGetValue("lessons", out IEnumerable<LessonViewModel>? model))
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
        [Route("Read/{id}")]
        public async Task<IActionResult> Read(Guid id)
        {
            if (!await lessonService.ExistsById(id))
            {
                return BadRequest();
            }

            await lessonService.Read(id, User.Id());
            EditLessonViewModel model = lessonService.ToEdit(await lessonService.GetById(id));
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!User.IsTeacher())
            {
                return Unauthorized();
            }

            return RedirectToAction("Edit", new EditLessonViewModel());
        }

        [HttpGet]
        [Route("Lessons/Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, EditLessonViewModel model)
        {
            if (!User.IsTeacher())
            {
                return RedirectToAction("Read", new { id = id });
            }

            if (model == null)
            {
                Guid teacherId = await teacherService.GetTeacherId(User.Id());

                if (!await teacherService.IsLessonCreator(id, teacherId))
                {
                    return RedirectToAction("Read", new { id = id });
                }

                model = lessonService.ToEdit(await lessonService.GetById(id));
            }
            return View(model);
        }

        [HttpGet]
        [Route("Lessons/SubmitEdit/{id}")]
        public async Task<IActionResult> SubmitEdit(Guid id, string title, string content, string htmlContent, string school, string subject, int grade)
        {
            EditLessonViewModel model = new EditLessonViewModel()
            {
                Id = id,
                Title = title,
                Content = content,
                HtmlContent = htmlContent,
                School = school,
                Subject = subject,
                Grade = grade
            };
            if (grade < 1 || grade > 12)
            {
                return Content("Класът трябва да е между 1 и 12");
            }
            if (!ModelState.IsValid)
            {
                return Content(string.Join('\n', ModelState.Values.SelectMany(v => v.Errors).Select(e=>e.ErrorMessage)));
            }
            if (!User.IsTeacher())
            {
                return Unauthorized();
            }
            if (!await lessonService.ExistsById(id))
            {
                await lessonService.Create(model, await teacherService.GetTeacherId(User.Id()));
                return RedirectToAction("Index");
            }

            Guid teacherId = await teacherService.GetTeacherId(User.Id());

            if (!await teacherService.IsLessonCreator(id, teacherId))
            {
                return NotFound();
            }

            await lessonService.Edit(id, model);
            return Content($"/Lessons/Read/{id}");
        }

        [HttpGet]
        public async Task<IActionResult> Like(Guid lessonId, string userId)
        {
            await lessonService.LikeLesson(lessonId, userId);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> Unlike(Guid lessonId, string userId)
        {
            await lessonService.UnlikeLesson(lessonId, userId);
            return NoContent();
        }
    }
}
