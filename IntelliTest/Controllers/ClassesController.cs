using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Classes;
using IntelliTest.Core.Models.Users;
using IntelliTest.Data.Entities;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

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
            if (false && cache.TryGetValue("tests", out IEnumerable<ClassViewModel>? model))
            {
            }
            else
            {
                model = await classService.GetAll(User.Id(), User.IsStudent(), User.IsTeacher());
                //var cacheEntryOptions = new DistributedCacheEntryOptions()
                //    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                cache.SetAsync("tests", model, cacheEntryOptions);
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
            if (TempData.Peek("TeacherId") is null)
            {
                return RedirectToAction("Logout", "User");
            }
            model.Teacher = new TeacherViewModel()
            {
                Id = (Guid)TempData.Peek("TeacherId")
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

            if (TempData.Peek("TeacherId") is null)
            {
                return RedirectToAction("Logout", "User");
            }
            model.Teacher = new TeacherViewModel()
            {
                Id = (Guid)TempData.Peek("TeacherId")
            };
            await classService.Create(model);
            TempData["message"] = "Успешно създаден клас";
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
                string serverFolder = Path.Combine(webHostEnvironment.WebRootPath, folder);
                await model.Image.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }

            await classService.Edit(model, id);
            TempData["message"] = "Успешно редактиран клас";
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
            TempData["message"] = "Успешно изтрит клас";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Details/{Id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            if (await classService.IsInClass(id, User.Id(), User.IsStudent(), User.IsTeacher()))
            {
                
            }

            var tests = await testResultsService.TestsTakenByClass(id);
            if (tests == null)
            {
                return NotFound();
            }

            var students = await classService.GetClassStudents(id);
            var classModel = await classService.GetById(id);
            return View("Details", new ClassDetailsModel()
            {
                Description = classModel.Description,
                Name = classModel.Name,
                Students = students,
                Tests = tests,
                Id = id
            });
        }
        [HttpPost]
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

            return View("Details");
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
        public IActionResult Join()
        {
            if (User.IsTeacher())
            {
                return Unauthorized();
            }
            return View();
        }
        [HttpPost]
        //[Route("Join/{Id}")]
        public async Task<IActionResult> Join(JoinModel model)
        {
            if (User.IsTeacher())
            {
                return Unauthorized();
            }

            if (TempData.Peek("StudentId") is null)
            {
                return RedirectToAction("Logout", "User");
            }
            bool success = await classService.AddStudent((Guid)TempData.Peek("StudentId"), model.Id);
            if (!success)
            {
                ModelState.AddModelError("Id", "Курсът не е намерен");
            }
            return View( new JoinModel()
            {
                Id = model.Id
            });
        }
    }
}
