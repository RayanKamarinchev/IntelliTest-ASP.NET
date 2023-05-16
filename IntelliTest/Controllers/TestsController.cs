using System.Diagnostics;
using System.Text.Json;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Data.Enums;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace IntelliTest.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        const string SCRIPT_NAME = "script.py";
        private readonly ITestService testService;
        //private readonly IDistributedCache cache;
        private readonly IMemoryCache cache;
        private readonly IStudentService studentService;
        private readonly ITeacherService teacherService;
        private readonly IClassService classService;

        public TestsController(ITestService _testService, IMemoryCache _cache, IStudentService _studentService,
                               ITeacherService _teacherService, IClassService _classService)
        {
            testService = _testService;
            cache = _cache;
            studentService = _studentService;
            teacherService = _teacherService;
            classService = _classService;
        }

        [HttpGet]
        public IActionResult GetFilter(Filter model)
        {
            return PartialView("FilterMenuPartialView", model);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string SearchTerm, int Grade, Subject Subject, Sorting Sorting, int currentPage)
        {
            if (1==0 && cache.TryGetValue("tests", out QueryModel<TestViewModel>? model))
            {
            }
            else
            {
                if (currentPage==0)
                {
                    currentPage = 1;
                }
                QueryModel<TestViewModel> query = new QueryModel<TestViewModel>(SearchTerm, Grade, Subject, Sorting, currentPage);
                model = await testService.GetAll(User.IsTeacher(), query);
                //var cacheEntryOptions = new DistributedCacheEntryOptions()
                //    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                cache.SetAsync("tests", model, cacheEntryOptions);
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> MyTests([FromQuery] QueryModel<TestViewModel> query)
        {
            QueryModel<TestViewModel> model = new QueryModel<TestViewModel>();
            Guid teacherId = await teacherService.GetTeacherId(User.Id());
            model = await testService.GetMy(teacherId, query);
            return View("Index", model);
        }
        

        [Route("Tests/Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, TestEditViewModel viewModel)
        {
            if (viewModel.PublicityLevel!=PublicityLevel.Link && viewModel.PublicityLevel != PublicityLevel.Public)
            {
                if (viewModel.PublicityLevel == PublicityLevel.TeachersOnly)
                {
                    if (!User.IsTeacher())
                    {
                        return Unauthorized();
                    }
                }
                else if (viewModel.PublicityLevel == PublicityLevel.ClassOnly)
                {
                    Guid testId = id;
                    Guid studentId = await studentService.GetStudentId(User.Id());
                    if (id == new Guid("257a4a2e-42cd-4180-ae5d-5b74a9f55b14"))
                    {
                        testId = (Guid)TempData.Peek("testId");
                    }

                    bool studentHasAccess = await testService.StudentHasAccess(testId, studentId);
                    if (!studentHasAccess)
                    {
                        return Unauthorized();
                    }
                }

                else if (viewModel.PublicityLevel == PublicityLevel.ClassOnly)
                {
                    if (!User.IsTeacher())
                    {
                        return Unauthorized();
                    }

                    Guid testId = id;
                    if (id == new Guid("257a4a2e-42cd-4180-ae5d-5b74a9f55b14"))
                    {
                        testId = (Guid)TempData.Peek("testId");
                    }
                    Guid teacherId = await teacherService.GetTeacherId(User.Id());
                    bool isCreator = await teacherService.IsTestCreator(testId, teacherId);
                    if (!isCreator)
                    {
                        return Unauthorized();
                    }
                }
            }
            if (!await testService.ExistsbyId(id))
            {
                return BadRequest();
            }
            var model = await testService.GetById(id);
            var testEdit = testService.ToEdit(model);
            testEdit.Id = id;
            TempData["PublicityLevel"] = testEdit.PublicityLevel;
            return View("Edit", testEdit);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Edit([FromBody]TestEditViewModel model)
        {
            if (model.Id is null)
            {
                return View("Edit", model);
            }
            if (!await testService.ExistsbyId(model.Id.Value))
            {
                return NotFound();
            }

            if (!model.ClosedQuestions.All(c => c.AnswerIndexes.Any(ai => ai)))
            {
                return View("Edit", model);
            }
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            model.PublicityLevel = (PublicityLevel)TempData["PublicityLevel"];
            Guid teacherId = await teacherService.GetTeacherId(User.Id());
            await testService.Edit(model.Id.Value, model, teacherId);
            TempData["message"] = "Успешно редактира тест!";
            return Content("redirect");
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create()
        {
            TempData["Classes"] = (await classService.GetAll(User.Id(), User.IsStudent(), User.IsTeacher()))
                                  .Select(c => c.Name).ToList();
            return View("Create", new TestViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(TestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Guid teacherId = await teacherService.GetTeacherId(User.Id());
            string[] allClasses = (string[])TempData["Classes"];
            string[] classNames = allClasses.Where((c, i) => model.Selected[i]).ToArray();
            Guid id = await testService.Create(model, teacherId, classNames);
            return RedirectToAction("Edit", new {id = id});
        }

        [HttpGet]
        [Route("Take/{testId}")]
        public async Task<IActionResult> Take(Guid testId)
        { 
            if (!User.IsStudent())
            {
                return Unauthorized();
            }
            if (!await testService.ExistsbyId(testId))
            {
                return NotFound();
            }
            if (await testService.IsTestTakenByStudentId(testId, await studentService.GetStudent(await studentService.GetStudentId(User.Id()))))
            {
                return BadRequest();
            }

            if (User.IsTeacher())
            {
                if (await teacherService.IsTestCreator(testId, await teacherService.GetTeacherId(User.Id())))
                {
                    return NotFound();
                }
            }

            var test = testService.ToSubmit(await testService.GetById(testId));
            return View(test);
        }

        [HttpPost]
        [Route("Take/{testId}")]
        public async Task<IActionResult> Take(TestSubmitViewModel model, Guid testId)
        {
            Guid studentId = await studentService.GetStudentId(User.Id());
            await testService.AddTestAnswer(model.OpenQuestions, model.ClosedQuestions, studentId, testId);
            TempData["message"] = "Успешно предаде теста!";
            return RedirectToAction("ReviewAnswers", new { testId = testId, studentId = studentId });
        }

        [HttpGet]
        [Route("Review/{testId}-{studentId}")]
        public async Task<IActionResult> ReviewAnswers(Guid testId, Guid studentId)
        {
            if (!User.IsStudent())
            {
                return Unauthorized();
            }
            if (!await testService.ExistsbyId(testId))
            {
                return BadRequest();
            }

            if (studentId != await studentService.GetStudentId(User.Id()))
            {
                return Unauthorized();
            }

            var test = await testService.TestResults(testId, studentId);
            return View(test);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Statistics(Guid testId)
        {
            if (!await teacherService.IsTestCreator(testId, await teacherService.GetTeacherId(User.Id())))
            {
                return NotFound();
            }

            var model = await testService.GetStatistics(testId);

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        [Route("Test/Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            Guid teacherId = await teacherService.GetTeacherId(User.Id());
            if (!await teacherService.IsTestCreator(id, teacherId))
            {
                return NotFound();
            }
            await testService.DeleteTest(id);
            TempData["message"] = "Успешно изтри тест";
            return RedirectToAction("ViewProfile", "User");
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult AddQuestion(OpenQuestionViewModel question)
        {
            var model = JsonSerializer.Deserialize<TestEditViewModel>(TempData.Peek("editModel").ToString());
            model.OpenQuestions.Add(question);
            TempData["editModel"] = JsonSerializer.Serialize(model);
            return View("Edit", model);
        }
    }
}
