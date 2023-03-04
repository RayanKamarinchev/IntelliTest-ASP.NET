using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Services;
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
        OpenQuestionAdd, ClosedQuestionAdd, Normal
    }

    [Authorize]
    public class TestsController : Controller
    {
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
        public async Task<IActionResult> Edit(int id, EditType type, TestEditViewModel viewModel)
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
                        Answers = new[] { "" },
                        Order = model.ClosedQuestions.Count + model.OpenQuestions.Count
                    };
                    model.ClosedQuestions.Add(q);
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
            var res = run_cmd("script.py", "Войната приключва с подписването на предварителния Санстефански мирен договор на 3 март 1878 г. Той учредява автономно княжество България, което обхваща ядрото на българските земи в Мизия, Тракия и Македония (без Северна Добруджа, предадена на Румъния, и Нишко – на Сърбия). Европейските държави отхвърлят договора, защото се опасяват, че обширното ново княжество ще изпадне под пълно руско влияние.На 1 юли 1878 г. в Берлин се свиква конгрес, на който Великите сили разпокъсват българските земи на 5 части. Румъния се разширява в Северна Добруджа. Освен Нишко Сърбия получава и Пиротско. Султанът си връща цяла Македония и Одринска Тракия. Между Дунав и Стара планина е създадено васално княжество България, а на юг – автономна провинция Източна Румелия. Тези решения пораждат у българите желание за съпротива и за обединение на разпокъсаните земи.");
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
