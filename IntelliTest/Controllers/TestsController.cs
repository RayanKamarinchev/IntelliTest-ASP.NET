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
        private readonly ITestService testService;
        //private readonly IDistributedCache cache;
        private readonly IMemoryCache cache;
        private readonly IStudentService studentService;
        private readonly IClassService classService;

        public TestsController(ITestService _testService, IMemoryCache _cache, IStudentService _studentService, IClassService _classService)
        {
            testService = _testService;
            cache = _cache;
            studentService = _studentService;
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
            if (cache.TryGetValue("tests", out QueryModel<TestViewModel>? model))
            {
            }
            else
            {
                if (currentPage==0)
                {
                    currentPage = 1;
                }
                QueryModel<TestViewModel> query = new QueryModel<TestViewModel>(SearchTerm, Grade, Subject, Sorting, currentPage);
                model = await testService.GetAll((Guid?)TempData.Peek("TeacherId") ?? null, (Guid?)TempData.Peek("StudentId") ?? null, query);
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
            model = await testService.GetMy((Guid?)TempData.Peek("TeacherId") ?? null, (Guid?)TempData.Peek("StudentId") ?? null, query);
            return View("Index", model);
        }
        

        [Route("Tests/Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id, TestEditViewModel viewModel)
        {
            if (!User.IsTeacher())
            {
                return Unauthorized();
            }
            if (viewModel.PublicityLevel!=PublicityLevel.Link && viewModel.PublicityLevel != PublicityLevel.Public)
            {
                if (viewModel.PublicityLevel == PublicityLevel.ClassOnly)
                {
                    Guid testId = id;
                    if (id == new Guid("257a4a2e-42cd-4180-ae5d-5b74a9f55b14"))
                    {
                        testId = (Guid)TempData.Peek("testId");
                    }
                    bool isCreator = await testService.IsTestCreator(testId, (Guid)TempData.Peek("TeacherId"));
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
            if (!User.IsTeacher())
            {
                return Unauthorized();
            }
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
            await testService.Edit(model.Id.Value, model, (Guid)TempData.Peek("TeacherId"));
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

            if (!User.IsTeacher())
            {
                return Unauthorized();
            }
            string[] allClasses = (string[])TempData["Classes"];
            string[] classNames = allClasses.Where((c, i) => model.Selected[i]).ToArray();
            Guid id = await testService.Create(model, (Guid)TempData.Peek("TeacherId"), classNames);
            return RedirectToAction("Edit", new {id = id});
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("Take/{testId}")]
        public async Task<IActionResult> Take(Guid testId)
        { 
            if (!await testService.ExistsbyId(testId))
            {
                return NotFound();
            }

            var studentId = (Guid)TempData.Peek("StudentId");
            if (await testService.IsTestTakenByStudentId(testId, studentId))
            {
                return RedirectToAction("ReviewAnswers");
            }

            var test = testService.ToSubmit(await testService.GetById(testId));
            return View(test);
        }

        [HttpPost]
        [Route("Take/{testId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Take(TestSubmitViewModel model, Guid testId)
        {
            if (!await testService.ExistsbyId(testId))
            {
                return NotFound();
            }

            var studentId = (Guid)TempData.Peek("StudentId");
            if (await testService.IsTestTakenByStudentId(testId, (Guid)TempData.Peek("StudentId")))
            {
                return RedirectToAction("ReviewAnswers");
            }
            await testService.AddTestAnswer(model.OpenQuestions, model.ClosedQuestions, studentId, testId);
            TempData["message"] = "Успешно предаде теста!";
            TempData["TakingTest"] = false;
            TempData.Remove("TestStart");
            return RedirectToAction("ReviewAnswers", new { testId = testId, studentId = studentId });
        }

        [HttpGet]
        [Route("Review/{testId}/{studentId}")]
        public async Task<IActionResult> ReviewAnswers(Guid testId, Guid studentId)
        {
            var teacherId = (Guid?)TempData.Peek("TeacherId") ?? null;
            var student = await studentService.GetStudent(studentId);
            bool isStudentsTeacher = student.Classes
                                            .Select(c => c.Class)
                                            .Select(c => c.Teacher)
                                            .Any(t => t.Id == teacherId);
            if (!User.IsStudent() && !isStudentsTeacher)
            {
                return Unauthorized();
            }
            if (!await testService.ExistsbyId(testId))
            {
                return NotFound();
            }

            if (TempData.Peek("StudentId") is null || studentId != (Guid)TempData.Peek("StudentId"))
            {
                return Unauthorized();
            }
            
            if (!await testService.IsTestTakenByStudentId(testId,(Guid)TempData.Peek("StudentId")))
            {
                return NotFound();
            }

            var test = await testService.TestResults(testId, studentId);
            return View(test);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Statistics(Guid testId)
        {
            if (!await testService.IsTestCreator(testId, (Guid)TempData.Peek("TeacherId")))
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
            if (!await testService.IsTestCreator(id, (Guid)TempData.Peek("TeacherId")))
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
