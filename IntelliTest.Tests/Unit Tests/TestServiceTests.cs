using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json;

namespace IntelliTest.Tests.Unit_Tests
{
    [TestFixture]
    public class TestServiceTests : UnitTestBase
    {
        private ITestService testService;
        private Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        private Guid id2 = Guid.Parse("fcda0a94-d7f6-4836-a093-f69066f177c7");

        [OneTimeSetUp]
        public void SetUp() =>
            testService = new TestService(data, configuration);

        [Test]
        public async Task GetById_Correct()
        {
            var test = await testService.GetById(id);
            var real = await data.Tests.FirstOrDefaultAsync(t => t.Id == id);
            Assert.AreEqual(real.Id, test.Id);
        }

        [Test]
        public async Task GetMy_Correct()
        {
            var test = await testService.GetMy(id, new QueryModel<TestViewModel>());
            var real = await data.Tests.FirstOrDefaultAsync(t => t.Id == id);
            Assert.AreEqual(test.Items.First().Id, real.Id);
        }

        private QueryModel<TestViewModel> query = new QueryModel<TestViewModel>()
        {
            CurrentPage = 1,
            Filters = new Filter()
            {
                Sorting = Sorting.Likes,
                Subject = Subject.Няма
            },
            ItemsPerPage = 3
        };

        [Test]
        public async Task GetAll_CorrectStudent()
        {
            var test = await testService.GetAll(false, query);
            var real = await data.Tests.FirstOrDefaultAsync(t => t.Id == id);
            Assert.AreEqual(0, test.Items.Count());
        }
        [Test]
        public async Task GetAll_CorrectTeacher()
        {
            var test = await testService.GetAll(true, query);
            var real = await data.Tests.FirstOrDefaultAsync(t => t.Id == id);
            Assert.AreEqual(test.Items.First().Id, real.Id);
        }
        [Test]
        public async Task ToEdit_Correct()
        {
            var testDb = await testService.GetById(id);
            var test = testService.ToEdit(testDb);
            var real = new TestEditViewModel()
            {
                Description = "Test Test",
                Grade = 8,
                Time = 10,
                Title = "The test",
                PublicityLevel = PublicityLevel.TeachersOnly,
                Id = id,
            };
            Assert.AreEqual(test.Id, real.Id);
            Assert.AreEqual(test.Description, real.Description);
            Assert.AreEqual(test.Grade, real.Grade);
            Assert.AreEqual(test.PublicityLevel, real.PublicityLevel);
            Assert.AreEqual(test.Time, real.Time);
            Assert.AreEqual(test.Title, real.Title);
        }
        [Test]
        public async Task ToSubmit_Correct()
        {
            var testDb = await testService.GetById(id);
            var test = testService.ToSubmit(testDb);
            var real = new TestSubmitViewModel()
            {
                Time = 10,
                Title = "The test",
                OpenQuestions = new List<OpenQuestionAnswerViewModel>()
                {
                    new OpenQuestionAnswerViewModel()
                    {
                        MaxScore = 3,
                        Id = id,
                        Text = "Who are you",
                        Order = 1,
                    },
                    new OpenQuestionAnswerViewModel()
                    {
                        Text = "How are you",
                        MaxScore = 1,
                        Order = 2,
                        Id = id2
                    }
                },
                ClosedQuestions = new List<ClosedQuestionAnswerViewModel>()
                {
                    new ClosedQuestionAnswerViewModel()
                    {
                        PossibleAnswers = new []{ "едно","две","три","четири" },
                        MaxScore = 2,
                        Order = 0,
                        Text = "Избери",
                        Id = id
                    }
                }
            };
            Assert.AreEqual(JsonConvert.SerializeObject(real), JsonConvert.SerializeObject(test));
        }

        [Test]
        public async Task Edit_Correct()
        {
            var testDb = await testService.GetById(id);
            var test = testService.ToEdit(testDb);
            test.Grade = 1;
            test.Description = "Another Description";
            test.PublicityLevel = PublicityLevel.Public;
            test.Time = 1;
            test.Title = "Another title";
            await testService.Edit(id, test, id);
            testDb = await testService.GetById(id);
            Assert.AreEqual(1, testDb.Grade);
            Assert.AreEqual("Another Description", testDb.Description);
            Assert.AreEqual(PublicityLevel.Public, testDb.PublicityLevel);
            Assert.AreEqual(1, testDb.Time);
            Assert.AreEqual("Another title", testDb.Title);
            SetUpBase();
            SetUp();
        }

        [Test]
        public async Task CalculateClosedQuestionScore_Correct()
        {
            Assert.AreEqual(1.5, testService.CalculateClosedQuestionScore(new []{false, true, false, false}, new []{1,3}, 3));
        }
        [Test]
        public async Task AddTestAnswer_Correct()
        {
            var OpenQuestions = new List<OpenQuestionAnswerViewModel>()
            {
                new OpenQuestionAnswerViewModel()
                {
                    MaxScore = 3,
                    Id = id,
                    Text = "Who are you",
                    Order = 1,
                    Answer = "Its me, Mario"
                },
                new OpenQuestionAnswerViewModel()
                {
                    Text = "How are you",
                    MaxScore = 1,
                    Order = 2,
                    Id = id2,
                    Answer = "Fine"
                }
            };
            var ClosedQuestions = new List<ClosedQuestionAnswerViewModel>()
            {
                new ClosedQuestionAnswerViewModel()
                {
                    PossibleAnswers = new[] { "едно", "две", "три", "четири" },
                    MaxScore = 2,
                    Order = 0,
                    Text = "Избери",
                    Id = id,
                    Answers = new []{false, true, false, true}
                }
            };
            data.ClosedQuestionAnswers.RemoveRange(data.ClosedQuestionAnswers);
            data.OpenQuestionAnswers.RemoveRange(data.OpenQuestionAnswers);
            await data.SaveChangesAsync();
            await testService.AddTestAnswer(OpenQuestions, ClosedQuestions, id, id);
            Assert.AreEqual(2, data.OpenQuestionAnswers.Count());
            Assert.AreEqual(1, data.ClosedQuestionAnswers.Count());
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task ProccessAnswerIndexes_Correct()
        {
            Assert.AreEqual(new[]{false,true,false,true}, testService.ProccessAnswerIndexes(new[]{"one", "two", "three", "four"},"1&3"));
        }
        [Test]
        public async Task Translate_Correct()
        {
            Assert.AreEqual("In Europe, the New Age began at the end of the 15th century and continued until the First World War (1914 - 1918).", testService.Translate("В Европа Новото време започва от края на XV в. и продължава до Първата световна война (1914 – 1918 г.)."));
        }
        [Test]
        public async Task IsTestTakenByStudentId_Correct()
        {
            Assert.IsTrue(await testService.IsTestTakenByStudentId(id, await data.Students.FirstOrDefaultAsync()));
        }
        [Test]
        public async Task GetExaminersIds_Correct()
        {
            Assert.AreEqual(id, testService.GetExaminersIds(id)[0]);
        }
        [Test]
        public async Task TestsTakenByStudent_Correct()
        {
            var test = (await testService.TestsTakenByStudent(id, query)).Items.FirstOrDefault();
            string json1 = JsonConvert.SerializeObject(await testService.GetById(id), Formatting.Indented,
                                                       new JsonSerializerSettings()
                                                       {
                                                           ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling
                                                                                             .Ignore
                                                       });
            string json2 = JsonConvert.SerializeObject(test, Formatting.Indented,
                                                       new JsonSerializerSettings()
                                                       {
                                                           ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                       });
            Assert.AreEqual(json1, json2);
        }
        [Test]
        public async Task TestsTaken_Correct()
        {
            var test = (await testService.TestsTakenByStudent(id, query)).Items.FirstOrDefault();
            string json1 = JsonConvert.SerializeObject(await testService.GetById(id), Formatting.Indented,
                                                       new JsonSerializerSettings()
                                                       {
                                                           ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling
                                                                                             .Ignore
                                                       });
            string json2 = JsonConvert.SerializeObject(test, Formatting.Indented,
                                                       new JsonSerializerSettings()
                                                       {
                                                           ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                       });
            Assert.AreEqual(json1, json2);
        }
    }
}
