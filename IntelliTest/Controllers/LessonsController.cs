using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Lessons;
using IntelliTest.Data.Enums;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace IntelliTest.Controllers
{
    [Authorize]
    public class LessonsController : Controller
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
            if (User.IsAdmin())
            {
                return RedirectToAction("Index", "Lessons", new { area = "Admin" });
            }
            if (cache.TryGetValue("lessons", out QueryModel<LessonViewModel>? model) && false)
            {
            }
            else
            {
                if (currentPage == 0)
                {
                    currentPage = 1;
                }
                QueryModel<LessonViewModel> query = new QueryModel<LessonViewModel>(SearchTerm, Grade, Subject, Sorting, currentPage);
                var teacherId = (Guid?)TempData.Peek("TeacherId") ?? null;
                model = await lessonService.GetAll(teacherId, query, User.Id());
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                cache.SetAsync("lessons", model, cacheEntryOptions);
            }
            return View(model);
        }

        [HttpGet]
        [Route("Read/{Id}")]
        public async Task<IActionResult> Read(Guid id)
        {
            Guid? teacherId = null;
            if (TempData.Peek("TeacherId") is not null)
            {
                teacherId = (Guid)TempData.Peek("TeacherId");
            }
            if (!User.IsAdmin() && !await lessonService.ExistsById(teacherId, id))
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
            if (!User.IsTeacher() && !User.IsAdmin())
            {
                return Unauthorized();
            }

            return RedirectToAction("Edit", new EditLessonViewModel());
        }

        [HttpGet]
        [Route("Lessons/Edit/{Id}")]
        public async Task<IActionResult> Edit(Guid id, EditLessonViewModel model)
        {
            if (!User.IsTeacher() || User.IsAdmin())
            {
                return RedirectToAction("Read", new { id = id });
            }

            if (model is null || model.Title is null)
            {
                if (TempData.Peek("TeacherId") is null && !User.IsAdmin())
                {
                    return RedirectToAction("Logout", "User");
                }

                bool isLessonCreator = await lessonService.IsLessonCreator(id, (Guid)TempData.Peek("TeacherId"));
                if (!User.IsAdmin() && !(isLessonCreator || id == Guid.Empty))
                {
                    return RedirectToAction("Read", new { id = id });
                }

                var lessonDb = await lessonService.GetById(id);
                if (lessonDb is null)
                {
                    lessonDb = new LessonViewModel()
                    {
                        Content = "",
                        Title = "",
                        School = "",
                        Grade = 0,
                        Id = Guid.Empty,
                        HtmlContent = ""
                    };
                }
                model = lessonService.ToEdit(lessonDb);
            }
            return View(model);
        }

        [HttpPost]
        [Route("Lessons/SubmitEdit/{Id}")]
        public async Task<IActionResult> SubmitEdit(Guid id, [FromBody] EditLessonViewModel model)
        {
            if (model.Grade < 1 || model.Grade > 12)
            {
                return Content("Класът трябва да е между 1 и 12");
            }
            if (!ModelState.IsValid)
            {
                return Content(string.Join('\n', ModelState.Values.SelectMany(v => v.Errors).Select(e=>e.ErrorMessage)));
            }
            if (!User.IsTeacher() && !User.IsAdmin())
            {
                return Unauthorized();
            }

            if (TempData.Peek("TeacherId") is null && !User.IsAdmin())
            {
                return RedirectToAction("Logout", "User");
            }
            if (!User.IsAdmin() && !await lessonService.ExistsById((Guid)TempData.Peek("TeacherId"), id))
            {
                await lessonService.Create(model, (Guid)TempData.Peek("TeacherId"));
                return Content("/Lessons/Index");
            }

            if (!User.IsAdmin() && !await lessonService.IsLessonCreator(id, (Guid)TempData.Peek("TeacherId")))
            {
                return NotFound();
            }

            await lessonService.Edit(id, model);
            return Content($"/Read/{id}");
        }

        [HttpGet]
        public async Task<IActionResult> Like(Guid lessonId, string userId)
        {
            if (!User.IsAdmin() && TempData.Peek("TeacherId") is null)
            {
                return RedirectToAction("Logout", "User");
            }
            if (!User.IsAdmin() && !await lessonService.ExistsById((Guid)TempData.Peek("TeacherId"), lessonId))
            {
                return NotFound();
            }
            await lessonService.LikeLesson(lessonId, userId);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> Unlike(Guid lessonId, string userId)
        {
            if (!User.IsAdmin() && TempData.Peek("TeacherId") is null)
            {
                return RedirectToAction("Logout", "User");
            }
            if (!User.IsAdmin() && !await lessonService.ExistsById((Guid)TempData.Peek("TeacherId"), lessonId))
            {
                return NotFound();
            }
            await lessonService.UnlikeLesson(lessonId, userId);
            return NoContent();
        }
    }
}
