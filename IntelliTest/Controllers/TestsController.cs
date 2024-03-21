using System.Text.Json;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Questions.Closed;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models.Tests.Groups;
using IntelliTest.Core.Models.Users;
using IntelliTest.Data.Enums;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using static IntelliTest.Infrastructure.Constraints;
using TestGroupSubmitViewModel = IntelliTest.Core.Models.Tests.Groups.TestGroupSubmitViewModel;

namespace IntelliTest.Controllers
{
    [Authorize]
    public class TestsController : Controller
    {
        private readonly ITestService testService;
        private readonly ITestResultsService testResultsService;
        private readonly IMemoryCache cache;
        private readonly IStudentService studentService;
        private readonly IClassService classService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly Random random = new Random();

        public TestsController(ITestService _testService, IMemoryCache _cache, IStudentService _studentService,
            IClassService _classService, ITestResultsService testResultsService, IWebHostEnvironment webHostEnvironment)
        {
            testService = _testService;
            cache = _cache;
            studentService = _studentService;
            classService = _classService;
            this.testResultsService = testResultsService;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult GetFilter(Filter model)
        {
            return PartialView("FilterMenuPartialView", model);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string SearchTerm, int Grade, Subject Subject, Sorting Sorting,
            int currentPage)
        {
            if (User.IsAdmin())
            {
                return RedirectToAction("Index", "Tests", new { area = AdminArea });
            }

            if (cache.TryGetValue(TestsCacheKey, out QueryModel<TestViewModel>? model) && false)
            {
            }
            else
            {
                if (currentPage == 0)
                {
                    currentPage = 1;
                }

                QueryModel<TestViewModel> query =
                    new QueryModel<TestViewModel>(SearchTerm, Grade, Subject, Sorting, currentPage);
                model = await testService.GetAll((Guid?)TempData.Peek(TeacherId) ?? null,
                    (Guid?)TempData.Peek(StudentId) ?? null, query);
                //var cacheEntryOptions = new DistributedCacheEntryOptions()
                //    .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                cache.SetAsync(TestsCacheKey, model, cacheEntryOptions);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> MyTests([FromQuery] QueryModel<TestViewModel> query)
        {
            Guid? teacherId = (Guid?)TempData.Peek(TeacherId);
            Guid? studentId = (Guid?)TempData.Peek(StudentId);
            QueryModel<TestViewModel> model = await testService.GetMy(teacherId, studentId, query);
            return View("Index", model);
        }

        [Route("Tests/Delete/{testId}/{groupId}")]
        public async Task<IActionResult> DeleteGroup(Guid testId, Guid groupId)
        {
            bool isCreator = await testService.IsTestCreator(testId, (Guid)TempData.Peek(TeacherId)) || User.IsAdmin();
            if (!await testService.TestExistsbyId(testId)
                || !await testService.GroupExistsbyId(groupId)
                || !isCreator)
            {
                return NotFound();
            }
            var groups = await testService.GetGroupsByTest(testId);

            if (groups.Count <= 1)
            {
                return BadRequest();
            }

            await testService.DeleteGroup(groupId);
            return RedirectToAction("Edit", new {testId = testId});
        }

        [Route("Tests/Edit/{testId}")]
        [Route("Tests/Edit/{testId}/{groupId}")]
        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Edit(TestGroupEditViewModel viewModel, Guid testId, Guid? groupId)
        {
            //TODO admin edit not found
            if (!await testService.TestExistsbyId(testId)
                || !await testService.GroupExistsbyId(groupId))
            {
                return NotFound();
            }

            if (viewModel.PublicityLevel == PublicityLevel.ClassOnly ||
                viewModel.PublicityLevel == PublicityLevel.Private)
            {
                //TODO potential bugs with TempData[testId]
                if (TempData.Peek(TeacherId) is null)
                {
                    return RedirectToAction("Logout", "User");
                }

                bool isCreator = await testService.IsTestCreator(testId, (Guid)TempData.Peek(TeacherId));
                if (!isCreator)
                {
                    return NotFound();
                }
            }

            TestViewModel testModel = await testService.GetById(testId);

            groupId ??= testModel.Groups.Last().Id;
            RawTestGroupViewModel groupModel = await testService.GetGroupById((Guid)groupId);
            TestGroupEditViewModel testToEdit = testResultsService.ToEdit(testModel, groupModel);

            TempData["PublicityLevel"] = testToEdit.PublicityLevel;
            TempData["TestGroups"] = JsonSerializer.Serialize(testToEdit.Groups);
            return View("Edit", testToEdit);
        }

        private async Task SaveImage(QuestionViewModel question)
        {
            if (question.Image != null && question.Image.ContentType.StartsWith("image"))
            {
                if (question.ImagePath.StartsWith("imgs/"))
                {
                    string path = Path.Combine(webHostEnvironment.WebRootPath, question.ImagePath);
                    System.IO.File.Delete(path);
                }

                string folder = "imgs/";
                folder += Guid.NewGuid() + "_" + question.Image.FileName;
                question.ImagePath = "/" + folder;
                string serverFolder = Path.Combine(webHostEnvironment.WebRootPath, folder);
                await question.Image.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }
        }

        [Route("Tests/Edit/AddGroup/{testId}")]
        public async Task<IActionResult> AddGroup(Guid testId, int groupNumber)
        {
            Guid groupId = await testService.AddNewGroup(testId, groupNumber);
            return RedirectToAction("Edit", new { testId = testId, groupId = groupId });
        }

        [HttpPost]
        [Route("Tests/Edit/{testId}/{groupId}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Edit(Guid testId, Guid groupId, [FromForm] TestGroupEditViewModel model)
        {
            model.OpenQuestions ??= new List<OpenQuestionViewModel>();
            model.ClosedQuestions ??= new List<ClosedQuestionViewModel>();
            model.QuestionsOrder ??= new List<QuestionType>();
            model.GroupId = groupId;
            model.Groups =
                JsonSerializer.Deserialize<List<TestGroupViewModel>>(TempData["TestGroups"].ToString());

            if (!await testService.TestExistsbyId(testId))
            {
                return NotFound();
            }

            foreach (var question in model.OpenQuestions)
            {
                await SaveImage(question);
            }

            foreach (var question in model.ClosedQuestions)
            {
                await SaveImage(question);
            }

            //test groups are counted as an error
            //TODO find smarter way
            if (ModelState.ErrorCount > 1 || !AllQuestionsHaveAnswerIndexes(model.ClosedQuestions) ||
                TempData.Peek("PublicityLevel") is null)
            {
                return View("Edit", model);
            }

            model.PublicityLevel = (PublicityLevel)TempData["PublicityLevel"];
            await testService.Edit(testId, model, (Guid?)TempData.Peek(TeacherId), User.IsAdmin());
            TempData.Remove("Groups");
            TempData[Message] = TestEditMsg;
            return Content("redirect");
        }

        private bool AllQuestionsHaveAnswerIndexes(List<ClosedQuestionViewModel> closedQuestions)
        {
            if (closedQuestions is null)
            {
                return true;
            }

            return closedQuestions.All(c => c.AnswerIndexes.Any(ai => ai));
        }


        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Create()
        {
            var classes = (await classService.GetAll(User.Id(), User.IsStudent(), User.IsTeacher()))
                          .Select(c => c.Name)
                          .ToArray();
            if (classes is null)
            {
                classes = new string[0];
            }

            TempData[Classes] = classes;
            return View("Create", new TestViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Create(TestViewModel model)
        {
            TempData[Classes] ??= new string[0];
            if (!(ModelState.ErrorCount == 1 && model.Selected is null))
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
            }

            if (TempData.Peek(TeacherId) is null && !User.IsAdmin())
            {
                return Unauthorized();
            }

            //model.Selected = new List<bool>();
            if (TempData.Peek(Classes) is null)
            {
                TempData[Classes] = new string[0];
            }

            string[] classNames = (string[])TempData.Peek(Classes);
            string[] selectedClasses = classNames.Where((c, i) => model.Selected[i]).ToArray();

            Guid id = await testService.Create(model, (Guid?)TempData.Peek(TeacherId) ?? AdminTeacherId,
                selectedClasses);
            return RedirectToAction("Edit", new { id = id });
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        [Route("Tests/Take/{testId}")]
        public async Task<IActionResult> Take(Guid testId)
        {
            if (!await testService.TestExistsbyId(testId))
            {
                return NotFound();
            }

            var studentId = (Guid)TempData.Peek(StudentId);
            if (await testService.IsTestTakenByStudentId(testId, studentId))
            {
                return RedirectToAction("ReviewAnswers", new { testId = testId, studentId = studentId });
            }

            var dbTest = await testService.GetById(testId);
            if (dbTest.Groups.Count == 0)
            {
                return BadRequest();
            }

            int index = random.Next(dbTest.Groups.Count);
            var group = dbTest.Groups.ToArray()[index];
            RawTestGroupViewModel model = new RawTestGroupViewModel()
                                          {
                                              ClosedQuestions = group.ClosedQuestions.ToList(),
                                              OpenQuestions = group.OpenQuestions.ToList(),
                                              Id = group.Id,
                                              TestId = testId,
                                              Number = group.Number,
                                              QuestionsOrder = group.QuestionsOrder,
                                              TestTitle = dbTest.Title,
                                              Time = dbTest.Time
                                          };
            var test = testService.ToSubmit(model);
            TempData["OpenQuestionIds"] = test.OpenQuestions.Select(q => q.Id.ToString()).ToArray();
            TempData["ClosedQuestionIds"] = test.ClosedQuestions.Select(q => q.Id.ToString()).ToArray();
            TempData["GroupId"] = group.Id;
            return View(test);
        }

        [HttpPost]
        [Route("Tests/Take/{testId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Take([FromBody] TestGroupSubmitViewModel model, Guid testId)
        {
            if (!await testService.TestExistsbyId(testId))
            {
                return NotFound();
            }

            var studentId = (Guid)TempData.Peek(StudentId);
            if (await testService.IsTestTakenByStudentId(testId, studentId))
            {
                return RedirectToAction("ReviewAnswers", new { testId = testId, studentId = studentId });
            }

            Guid groupId = (Guid)TempData["GroupId"];

            var openQuestionIds = (string[])TempData["OpenQuestionIds"];
            openQuestionIds ??= new string[0];
            for (int i = 0; i < openQuestionIds.Length; i++)
            {
                model.OpenQuestions[i].Id = new Guid(openQuestionIds[i]);
            }

            var closedQuestionIds = (string[])TempData["ClosedQuestionIds"];
            closedQuestionIds ??= new string[0];
            for (int i = 0; i < closedQuestionIds.Length; i++)
            {
                model.ClosedQuestions[i].Id = new Guid(closedQuestionIds[i]);
            }

            await testResultsService.SaveStudentTestAnswer(model.OpenQuestions, model.ClosedQuestions, studentId,
                groupId);

            TempData[Message] = TestSubmitMsg;
            TempData.Remove("TestStarted");
            return Ok("redirect");
        }

        [HttpGet]
        [Route("Tests/Review/{testId}/{studentId}")]
        public async Task<IActionResult> ReviewAnswers(Guid testId, Guid studentId)
        {
            var teacherId = (Guid?)TempData.Peek(TeacherId);
            var student = await studentService.GetStudent(studentId);
            bool isStudentsTeacher = student.Classes
                                            .Select(c => c.Class)
                                            .Select(c => c.Teacher)
                                            .Any(t => t.Id == teacherId);
            if (!User.IsAdmin())
            {
                if (!User.IsStudent() && !isStudentsTeacher)
                {
                    return Unauthorized();
                }

                if (!await testService.TestExistsbyId(testId))
                {
                    return NotFound();
                }

                if (TempData.Peek(StudentId) is null || studentId != (Guid)TempData.Peek(StudentId))
                {
                    return Unauthorized();
                }

                if (!await testService.IsTestTakenByStudentId(testId, (Guid)TempData.Peek(StudentId)))
                {
                    return NotFound();
                }
            }

            var test = await testResultsService.GetStudentTestResults(testId, studentId);
            return View(test);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> Statistics(Guid testId)
        {
            if (!(User.IsAdmin() || await testService.IsTestCreator(testId, (Guid)TempData.Peek(TeacherId))))
            {
                return NotFound();
            }

            var model = await testResultsService.GetStatistics(testId);

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        [Route("Test/Delete/{Id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!User.IsAdmin())
            {
                if (TempData.Peek(TeacherId) is null)
                {
                    return RedirectToAction("Logout", "User");
                }

                if (!await testService.IsTestCreator(id, (Guid)TempData.Peek(TeacherId)))
                {
                    return NotFound();
                }
            }

            await testService.DeleteTest(id);
            TempData[Message] = TestDeleteMsg;
            if (!User.IsAdmin())
                return RedirectToAction("Index", "Tests");
            return RedirectToAction("ViewProfile", "User");
        }

        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        public IActionResult AddQuestion(OpenQuestionViewModel question)
        {
            if (TempData.Peek("editModel") is null)
            {
                return View("Edit");
            }

            var model = JsonSerializer.Deserialize<TestGroupEditViewModel>(TempData.Peek("editModel").ToString());
            model.OpenQuestions.Add(question);
            TempData["editModel"] = JsonSerializer.Serialize(model);

            return View("Edit", model);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        [Route("Examiners/{testId}")]
        public async Task<IActionResult> ExaminersAll(Guid testId)
        {
            IEnumerable<StudentViewModel> examiners = await studentService.GetExaminers(testId);
            return View("ExaminersAll", examiners);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher,Admin")]
        [Route("Examiners/{testId}/{studentId}")]
        public async Task<IActionResult> TestGrading(Guid testId, Guid studentId)
        {
            //TODO
            var testResult = await testResultsService.GetStudentTestResults(testId, studentId);
            TempData["QuestionIds"] = testResult.OpenQuestions.Select(q => q.Id.ToString()).ToArray();
            TempData["TestId"] = testId;
            return View("TestGrading", testResult);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        [Route("Examiners/{testId}/{groupId}/{studentId}")]
        public async Task<IActionResult> TestGrading(Guid testId, Guid groupId, Guid studentId,
            TestReviewViewModel scoredTest)
        {
            if (!await testService.IsTestCreator(testId, (Guid)TempData.Peek(TeacherId)))
            {
                return NotFound();
            }

            if (TempData["QuestionIds"] is null || TempData["TestId"] is null || (Guid)TempData["TestId"] != testId)
            {
                return View("TestGrading", scoredTest);
            }

            string[] quesitonIds = (string[])TempData["QuestionIds"];
            for (int i = 0; i < quesitonIds.Length; i++)
            {
                scoredTest.OpenQuestions[i].Id = new Guid(quesitonIds[i]);
            }

            await testResultsService.SubmitTestScore(groupId, studentId, scoredTest);

            TempData.Remove("TestId");
            TempData["Message"] = "Успешно оценихте теста";
            return RedirectToRoute(new
                                   {
                                       controller = "Tests",
                                       action = "ExaminersAll",
                                       testId = testId
                                   });
        }
    }
}