using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Services;
using IntelliTest.Data.Entities;
using IntelliTest.Infrastructure;
using IntelliTest.Models.Tests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

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

        public TestsController(ITestService _testService, IDistributedCache _cache)
        {
            testService = _testService;
            cache = _cache;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            cache.Remove("tests");
            if (cache.TryGetValue("tests", out IEnumerable<TestViewModel>? model))
            {
            }
            else
            {
                model = await testService.GetAll();
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                                        .SetSlidingExpiration(TimeSpan.FromSeconds(60));
                await cache.SetAsync("tests", model, cacheEntryOptions);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> MyTests()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await testService.GetMy(userId);
            return View("Index", model);
        }
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, EditType type, TestEditViewModel viewModel, [FromForm] string text, int questionOrder)
        {
            if (id!=-1)
            {
                if (!testService.ExistsbyId(id))
                {
                    return BadRequest();
                }
                var model = await testService.GetById(id);
                var testEdit = testService.ToEdit(model);
                TempData["editModel"] = JsonSerializer.Serialize(testEdit);
                return View("Edit", testEdit);
            }
            else
            {
                var model = JsonSerializer.Deserialize<TestEditViewModel>(TempData.Peek("editModel").ToString());

                model.Description = viewModel.Description;
                model.Time = viewModel.Time;
                model.Grade = viewModel.Grade;
                model.Title = viewModel.Title;
                for (int i = 0; i < viewModel.OpenQuestions.Count; i++)
                {
                    model.OpenQuestions[i].Answer = viewModel.OpenQuestions[i].Answer;
                    model.OpenQuestions[i].Order = viewModel.OpenQuestions[i].Order;
                    model.OpenQuestions[i].Text = viewModel.OpenQuestions[i].Text;
                }
                for (int i = 0; i < viewModel.ClosedQuestions.Count; i++)
                {
                    model.ClosedQuestions[i].Answers = viewModel.ClosedQuestions[i].Answers;
                    model.ClosedQuestions[i].AnswerIndexes = viewModel.ClosedQuestions[i].AnswerIndexes;
                    model.ClosedQuestions[i].Order = viewModel.ClosedQuestions[i].Order;
                    model.ClosedQuestions[i].Text = viewModel.ClosedQuestions[i].Text;
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
                    var res = run_cmd(SCRIPT_NAME, text);
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
        public async Task<IActionResult> EditSubmit(int id, TestEditViewModel model)
        {
            if (!testService.ExistsbyId(id+1))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            await testService.Edit(id+1, model);
            return View("Index", await testService.GetAll());
        }

        public IActionResult AIGenerate(string text)
        {
            var res = run_cmd("script.py", text);
            return Json(JsonSerializer.Serialize(res));
        }

        private IEnumerable<string[]> run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\raian\AppData\Local\Programs\Python\Python38\python.exe";
            start.Arguments = string.Format("{0} \"{1}\"", cmd, args);
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

            all = all.Replace("<pad> въпрос: ", "");
            all = all.Replace("</s>", "");
            var res = all.Split("&").Select(qa => qa.Split("|"));
            //foreach (var qa in res)
            //{
            //    Console.WriteLine("Въпрос: ");
            //    Console.WriteLine(qa[0]);
            //    Console.WriteLine("Отговор: ");
            //    Console.WriteLine(qa[1]);
            //}
            return res;
        }
    }
}
