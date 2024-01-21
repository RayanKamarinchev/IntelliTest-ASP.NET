using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Classes;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models.Users;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static IntelliTest.Infrastructure.Constraints;

namespace IntelliTest.Controllers
{
    public class ClassesController : Controller
    {
        private readonly IClassService classService;
        private readonly ITestResultsService testResultsService;
        //private readonly IDistributedCache cache;
        private readonly IMemoryCache cache;
        private readonly IWebHostEnvironment webHostEnvironment;


        public ClassesController(IClassService _classService, IMemoryCache _cache,
                                 IWebHostEnvironment _webHostEnvironment, ITestResultsService testResultsService)
        {
            classService = _classService;
            cache = _cache;
            webHostEnvironment = _webHostEnvironment;
            this.testResultsService = testResultsService;
        }
        public async Task<IActionResult> Index()
        {
            if (User.IsAdmin())
            {
                return RedirectToAction("Index", "Classes", new { area = AdminArea });
            }
            if (cache.TryGetValue(ClassesCacheKey, out IEnumerable<ClassViewModel>? model))
            {
            }
            else
            {
                model = await classService.GetAll(User.Id(), User.IsStudent(), User.IsTeacher());
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                cache.SetAsync(ClassesCacheKey, model, cacheEntryOptions);
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
            if (TempData.Peek(TeacherId) is null)
            {
                return RedirectToAction("Logout", "User");
            }
            model.Teacher = new TeacherViewModel()
            {
                Id = (Guid)TempData.Peek(TeacherId)
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
                folder += Guid.NewGuid() + "_" + model.Image.FileName;
                model.ImageUrl = folder;
                string serverFolder = Path.Combine(webHostEnvironment.WebRootPath, folder);
                await model.Image.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }

            if (TempData.Peek(TeacherId) is null)
            {
                return RedirectToAction("Logout", "User");
            }
            model.Teacher = new TeacherViewModel()
            {
                Id = (Guid)TempData.Peek(TeacherId)
            };
            await classService.Create(model);
            TempData[Message] = ClassCreateMsg;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Edit/{Id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (User.IsStudent())
            {
                return Unauthorized();
            }

            var model = await classService.GetById(id);
            if (model == null)
            {
                return NotFound();
            }

            TempData["imagePath"] = model.ImageUrl;
            return View("Edit", model);
        }

        [HttpPost]
        [Route("Edit/{Id}")]
        public async Task<IActionResult> Edit(ClassViewModel model, Guid id)
        {
            if (User.IsStudent())
            {
                return Unauthorized();
            }

            if (!await classService.IsClassOwner(id, User.Id()))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Image != null && model.Image.ContentType.StartsWith("image"))
            {
                string folder = (string)TempData["imagePath"];
                if (folder == "")
                {
                    folder = "imgs/";
                    folder += Guid.NewGuid() + "_" + model.Image.FileName;
                }
                model.ImageUrl = folder;
                string serverFolder = Path.Combine(webHostEnvironment.WebRootPath, folder);
                await model.Image.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }

            await classService.Edit(model, id);
            TempData[Message] = ClassEditMsg;
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("Delete/{Id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (User.IsStudent())
            {
                return Unauthorized();
            }

            if (!await classService.IsClassOwner(id, User.Id()))
            {
                return NotFound();
            }

            await classService.Delete(id);
            TempData[Message] = ClassDeleteMsg;
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Details/{Id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            bool? isClassMember = await classService.IsInClass(id, User.Id(), User.IsStudent(), User.IsTeacher());
            if (isClassMember == null || !isClassMember.Value)
            {
                return NotFound();
            }

            var tests = await testResultsService.TestsTakenByClass(id) ?? new List<TestStatsViewModel>();

            var students = await classService.GetClassStudents(id);
            var classModel = await classService.GetById(id);
            return View("Details", new ClassDetailsModel()
            {
                Description = classModel.Description,
                Name = classModel.Name,
                Students = students,
                Tests = tests,
                Id = id,
                JoinCode = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/Join/{id}"
            });
        }
        [HttpGet]
        public async Task<IActionResult> RemoveStudent(Guid id,Guid studentId)
        {
            if (User.IsStudent())
            {
                return Unauthorized();
            }
            if (!await classService.IsClassOwner(id, User.Id()))
            {
                return NotFound();
            }

            bool success = await classService.RemoveStudent(studentId, id);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction("Details", new {id=id});
        }
        [HttpPost]
        public async Task<IActionResult> AddStudent(Guid id, Guid studentId)
        {
            if (User.IsStudent())
            {
                return Unauthorized();
            }
            if (!await classService.IsClassOwner(id, User.Id()))
            {
                return NotFound();
            }

            bool success = await classService.AddStudent(studentId, id);
            if (!success)
            {
                return NotFound();
            }

            return View("Details");
        }
        [HttpGet]
        [Route("Join/{Id}")]
        public async Task<IActionResult> Join(Guid Id)
        {
            if (User.IsTeacher())
            {
                return Unauthorized();
            }

            if (TempData.Peek(StudentId) is null)
            {
                return RedirectToAction("Logout", "User");
            }
            await classService.AddStudent((Guid)TempData.Peek(StudentId), Id);
            return RedirectToAction("Index");
        }
    }
}
