using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Lessons;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Services;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;
using IntelliTest.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace IntelliTest.Controllers
{
    public enum EditType
    {
        OpenQuestionAdd, ClosedQuestionAdd, AiGenerate, Delete
    }

    [Authorize]
    public class TestsController : Controller
    {
        const string SCRIPT_NAME = "script.py";
        private readonly ITestService testService;
        private readonly IDistributedCache cache;
        private readonly IStudentService studentService;
        private readonly ITeacherService teacherService;
        private readonly ILessonService lessonService;
        private readonly IClassService classService;

        public TestsController(ITestService _testService, IDistributedCache _cache, IStudentService _studentService,
                               ITeacherService _teacherService, ILessonService _lessonService, IClassService _classService)
        {
            testService = _testService;
            cache = _cache;
            studentService = _studentService;
            teacherService = _teacherService;
            lessonService = _lessonService;
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
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                await cache.SetAsync("tests", model, cacheEntryOptions);
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
        public async Task<IActionResult> Edit(Guid id, EditType type, TestEditViewModel viewModel, [FromForm] string text,string url, int questionOrder, bool isText)
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
            if (id!= new Guid("257a4a2e-42cd-4180-ae5d-5b74a9f55b14"))
            {
                if (!await testService.ExistsbyId(id))
                {
                    return BadRequest();
                }
                var model = await testService.GetById(id);
                var testEdit = testService.ToEdit(model);
                TempData["PublicityLevel"] = testEdit.PublicityLevel;
                TempData["editModel"] = JsonSerializer.Serialize(testEdit);
                TempData["testId"] = id;
                ModelState["Title"].Errors.Clear();
                return View("Edit", testEdit);
            }
            else
            {
                var model = JsonSerializer.Deserialize<TestEditViewModel>(TempData.Peek("editModel").ToString());

                model.Description = viewModel.Description;
                model.Time = viewModel.Time;
                model.Grade = viewModel.Grade;
                model.PublicityLevel = (PublicityLevel)Math.Max((int)viewModel.PublicityLevel, (int)TempData.Peek("PublicityLevel"));
                model.Title = viewModel.Title;
                if (viewModel.OpenQuestions != null)
                {
                    for (int i = 0; i < viewModel.OpenQuestions.Count; i++)
                    {
                        model.OpenQuestions[i].Answer = viewModel.OpenQuestions[i].Answer;
                        model.OpenQuestions[i].Order = viewModel.OpenQuestions[i].Order;
                        model.OpenQuestions[i].Text = viewModel.OpenQuestions[i].Text;
                        model.OpenQuestions[i].MaxScore = viewModel.OpenQuestions[i].MaxScore;
                    }
                }

                if (viewModel.ClosedQuestions != null)
                {
                    for (int i = 0; i < viewModel.ClosedQuestions.Count; i++)
                    {
                        model.ClosedQuestions[i].Answers = viewModel.ClosedQuestions[i].Answers;
                        model.ClosedQuestions[i].AnswerIndexes = viewModel.ClosedQuestions[i].AnswerIndexes;
                        model.ClosedQuestions[i].Order = viewModel.ClosedQuestions[i].Order;
                        model.ClosedQuestions[i].Text = viewModel.ClosedQuestions[i].Text;
                        model.ClosedQuestions[i].MaxScore = viewModel.ClosedQuestions[i].MaxScore;
                    }
                }

                if (type == EditType.OpenQuestionAdd)
                {
                    var q = new OpenQuestionViewModel()
                    {
                        Order = model.ClosedQuestions.Count + model.OpenQuestions.Count
                    };
                    model.OpenQuestions.Add(q);
                }
                else if (type == EditType.ClosedQuestionAdd)
                {
                    var q = new ClosedQuestionViewModel()
                    {
                        Answers = Enumerable.Repeat("", 6).ToArray(),
                        Order = model.ClosedQuestions.Count + model.OpenQuestions.Count
                    };
                    model.ClosedQuestions.Add(q);
                }
                else if (type == EditType.AiGenerate)
                {
                    if (!isText)
                    {
                        string guidString = url.Split('/').Last();
                        Guid guidResult;
                        bool isValid = Guid.TryParse(guidString, out guidResult);
                        if (isValid)
                        {
                            var lesson = await lessonService.GetById(guidResult);
                            if (lesson.IsPrivate)
                            {
                                ModelState.AddModelError("Title", "Урокът не е открит.");
                                TempData["editModel"] = JsonSerializer.Serialize(model);
                                return View("Edit", model);
                            }
                            text = lesson.Content;
                        }
                        else
                        {
                            var lesson = await lessonService.GetByName(url)!;
                            if (lesson==null)
                            {
                                ModelState.AddModelError("Title", "Урокът не е открит.");
                                TempData["editModel"] = JsonSerializer.Serialize(model);
                                return View("Edit", model);
                            }

                            text = lesson.Content;
                        }
                    }
                    var res = run_cmd(text);
                    foreach (var se in res)
                    {
                        var q = new OpenQuestionViewModel()
                        {
                            Order = model.ClosedQuestions.Count + model.OpenQuestions.Count,
                            Text = se[0],
                            Answer = se[1]
                        };
                        model.OpenQuestions.Add(q);
                    }
                }
                else if (type == EditType.Delete)
                {
                    var closedQuestion = model.ClosedQuestions.FirstOrDefault(q => q.Order == questionOrder);
                    int order; 
                    if (closedQuestion == null)
                    {
                        var openQuestion = model.OpenQuestions.FirstOrDefault(q => q.Order == questionOrder);
                        order = openQuestion.Order;
                        model.OpenQuestions.Remove(openQuestion);
                    }
                    else
                    {
                        order = closedQuestion.Order;
                        model.ClosedQuestions.Remove(closedQuestion);
                    }

                    for (int i = 0; i < model.OpenQuestions.Count; i++)
                    {
                        if (model.OpenQuestions[i].Order >= order)
                        {
                            model.OpenQuestions[i].Order--;
                        }
                    }
                    for (int i = 0; i < model.ClosedQuestions.Count; i++)
                    {
                        if (model.ClosedQuestions[i].Order >= order)
                        {
                            model.ClosedQuestions[i].Order--;
                        }
                    }
                }

                TempData["editModel"] = JsonSerializer.Serialize(model);
                return View("Edit", model);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> EditSubmit(Guid id, TestEditViewModel model)
        {
            if (model.ClosedQuestions == null)
            {
                model.ClosedQuestions = new List<ClosedQuestionViewModel>();
            }
            if (model.OpenQuestions == null)
            {
                model.OpenQuestions = new List<OpenQuestionViewModel>();
            }

            for (int i = 0; i < model.ClosedQuestions.Count; i++)
            {
                List<string> answers = new List<string>();
                List<bool> answersIndexes = new List<bool>();
                for (int j = 0; j < model.ClosedQuestions[i].Answers.Length; j++)
                {
                    if (model.ClosedQuestions[i].Answers[j]!=null)
                    {
                        answers.Add(model.ClosedQuestions[i].Answers[j]);
                        answersIndexes.Add(model.ClosedQuestions[i].AnswerIndexes[j]);
                    }
                }

                model.ClosedQuestions[i].Answers = answers.ToArray();
                model.ClosedQuestions[i].AnswerIndexes = answersIndexes.ToArray();
            }

            if (!await testService.ExistsbyId(id))
            {
                return NotFound();
            }

            if (!model.ClosedQuestions.All(c=>c.AnswerIndexes.Any(ai=>ai)))
            {
                return View("Edit", model);
            }
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            Guid teacherId = await teacherService.GetTeacherId(User.Id());
            await testService.Edit(id, model, teacherId);
            TempData["message"] = "Успешно редактира тест!";
            TempData.Remove("editModel");
            return RedirectToAction("Index");
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

        private IEnumerable<string[]> run_cmd(string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\raian\AppData\Local\Programs\Python\Python38\python.exe";
            start.Arguments = string.Format("{0} \"{1}\"", SCRIPT_NAME, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            string last = "";
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    last = result;
                }
            }

            string[] splitted = last.Split("*");
            string all = "";
            for (int i = 0; i < splitted.Length; i++)
            {
                all += (char)int.Parse(splitted[i]);
            }
            
            var res = all.Split("&").Select(qa => qa.Split("|"));
            return res;
        }
    }
}
