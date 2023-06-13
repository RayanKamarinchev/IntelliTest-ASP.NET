using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Lessons;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Services;
using IntelliTest.Data.Enums;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace IntelliTest.Controllers
{
    [Authorize]
    public class LessonsController : Controller
    {
        private readonly ILessonService lessonService;
        //private readonly IDistributedCache cache;
        private readonly IMemoryCache cache;

        public LessonsController(ILessonService _lessonService, IMemoryCache _cache)
        {
            lessonService = _lessonService;
            cache = _cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string SearchTerm, int Grade, Subject Subject, Sorting Sorting, int currentPage)
        {
            if (cache.TryGetValue("lessons", out QueryModel<LessonViewModel>? model))
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
                //var cacheEntryOptions = new DistributedCacheEntryOptions()
                //    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                cache.SetAsync("lessons", model, cacheEntryOptions);
            }
            return View(model);
        }

        [HttpGet]
        [Route("Read/{id}")]
        public async Task<IActionResult> Read(Guid id)
        {
            if (TempData.Peek("TeacherId") is null)
            {
                return RedirectToAction("Logout", "User");
            }
            if (!await lessonService.ExistsById((Guid)TempData.Peek("TeacherId"), id))
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
                if (TempData.Peek("TeacherId") is null)
                {
                    return RedirectToAction("Logout", "User");
                }
                if (!await lessonService.IsLessonCreator(id, (Guid)TempData.Peek("TeacherId")))
                {
                    return RedirectToAction("Read", new { id = id });
                }

                model = lessonService.ToEdit(await lessonService.GetById(id));
            }
            return View(model);
        }

        [HttpGet]
        [Route("Lessons/SubmitEdit/{id}")]
        public async Task<IActionResult> SubmitEdit(Guid id, string title, string content, string htmlContent, string school, Subject subject, int grade)
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

            if (TempData.Peek("TeacherId") is null)
            {
                return RedirectToAction("Logout", "User");
            }
            if (!await lessonService.ExistsById((Guid)TempData.Peek("TeacherId"), id))
            {
                await lessonService.Create(model, (Guid)TempData.Peek("TeacherId"));
                return RedirectToAction("Index");
            }

            if (!await lessonService.IsLessonCreator(id, (Guid)TempData.Peek("TeacherId")))
            {
                return NotFound();
            }

            await lessonService.Edit(id, model);
            return Content($"/Lessons/Read/{id}");
        }

        [HttpGet]
        public async Task<IActionResult> Like(Guid lessonId, string userId)
        {
            if (TempData.Peek("TeacherId") is null)
            {
                return RedirectToAction("Logout", "User");
            }
            if (!await lessonService.ExistsById((Guid)TempData.Peek("TeacherId"), lessonId))
            {
                return NotFound();
            }
            await lessonService.LikeLesson(lessonId, userId);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> Unlike(Guid lessonId, string userId)
        {
            if (TempData.Peek("TeacherId") is null)
            {
                return RedirectToAction("Logout", "User");
            }
            if (!await lessonService.ExistsById((Guid)TempData.Peek("TeacherId"), lessonId))
            {
                return NotFound();
            }
            await lessonService.UnlikeLesson(lessonId, userId);
            return NoContent();
        }
    }
}
