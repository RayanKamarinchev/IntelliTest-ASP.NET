using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Classes;
using IntelliTest.Core.Models.Users;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace IntelliTest.Controllers
{
    public class ClassesController : Controller
    {
        private readonly IClassService classService;
        private readonly ITeacherService teacherService;
        private readonly IDistributedCache cache;
        private readonly IWebHostEnvironment webHostEnvironment;


        public ClassesController(IClassService _classService, IDistributedCache _cache, ITeacherService _teacherService, IWebHostEnvironment _webHostEnvironment)
        {
            classService = _classService;
            cache = _cache;
            teacherService = _teacherService;
            webHostEnvironment = _webHostEnvironment;
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
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (User.IsStudent())
            {
                return Unauthorized();
            }
            var model = new ClassViewModel();
            model.Description = "";
            model.Name = "";
            model.Teacher = new TeacherViewModel()
            {
                Id = await teacherService.GetTeacherId(User.Id())
            };
            return View("Create", model);
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

            if (model.Image != null && model.Image.ContentType.StartsWith("image"))
            {
                string folder = "imgs/";
                folder += Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                model.ImageUrl = folder;
                string serverFolder = Path.Combine(webHostEnvironment.WebRootPath, folder);
                await model.Image.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }

            model.Teacher = new TeacherViewModel()
            {
                Id = await teacherService.GetTeacherId(User.Id())
            };
            await classService.Create(model);
            return RedirectToAction("Index");
        }
    }
}
