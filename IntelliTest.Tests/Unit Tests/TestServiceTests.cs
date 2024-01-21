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
using NUnit.Framework.Interfaces;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions.Closed;

namespace IntelliTest.Tests.Unit_Tests
{
    [TestFixture]
    public class TestServiceTests : UnitTestBase
    {
        private ITestService testService;
        private ITestResultsService testResultsService;
        private Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        private Guid id2 = Guid.Parse("fcda0a94-d7f6-4836-a093-f69066f177c7");

        [OneTimeSetUp]
        public void SetUp()
        {
            testResultsService = new TestResultsService(data, configuration);
            testService = new TestService(data);
        }

        public void SetUpQuery() => query = new QueryModel<TestViewModel>()
        {
            CurrentPage = 1,
            Filters = new Filter()
            {
                Sorting = Sorting.Likes,
                Subject = Subject.Няма
            },
            ItemsPerPage = 3
        };

        public TestViewModel GetTestByIdIncludingTestTaken(Guid id)
        {
            var t = data.Tests
                                 .Where(t => !t.IsDeleted)
                                 .Include(t => t.OpenQuestions)
                                 .Include(t => t.ClosedQuestions)
                                 .Include(t => t.TestResults)
                                 .Include(t => t.TestLikes)
                                 .FirstOrDefault(t => t.Id == id);
           return new TestViewModel()
            {
                AverageScore = Math.Round(!t.TestResults.Any() ? 0 : t.TestResults.Average(r => r.Score), 2),
                ClosedQuestions = t.ClosedQuestions,
                CreatedOn = t.CreatedOn,
                Description = t.Description,
                Grade = t.Grade,
                Id = t.Id,
                MaxScore = t.ClosedQuestions.Sum(q => q.MaxScore) +
                           t.OpenQuestions.Sum(q => q.MaxScore),
                OpenQuestions = t.OpenQuestions,
                Time = t.Time,
                Title = t.Title,
                MultiSubmit = t.MultiSubmission,
                PublicityLevel = t.PublicyLevel,
                Students = t.TestResults.Count(),
                IsTestTaken = t.TestResults.Any(t=>t.StudentId==id),
                QuestionOrder = t.QuestionsOrder
            };
        }

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
            var test = await testService.GetMy(id, null, new QueryModel<TestViewModel>());
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
            var test = await testService.GetAll(null, id, query);
            var real = await data.Tests.FirstOrDefaultAsync(t => t.Id == id);
            Assert.AreEqual(0, test.Items.Count());
        }
        [Test]
        public async Task GetAll_CorrectTeacher()
        {
            var test = await testService.GetAll(id, null, query);
            var real = await data.Tests.FirstOrDefaultAsync(t => t.Id == id);
            Assert.AreEqual(test.Items.First().Id, real.Id);
        }

        [Test]
        public async Task ToSubmit_Correct()
        {
            var testDb = await testService.GetById(id);
            var test = testService.ToSubmit(testDb);
            var real = new TestSubmitViewModel()
            {
                Id = test.Id,
                Time = 10,
                Title = "The test",
                OpenQuestions = new List<OpenQuestionViewModel>()
                {
                    new OpenQuestionViewModel()
                    {
                        MaxScore = 3,
                        Id = id,
                        Text = "Who are you",
                        ImagePath = ""
                    },
                    new OpenQuestionViewModel()
                    {
                        Text = "How are you",
                        MaxScore = 1,
                        Id = id2,
                        ImagePath = ""
                    }
                },
                ClosedQuestions = new List<ClosedQuestionViewModel>()
                {
                    new ClosedQuestionViewModel()
                    {
                        Answers = new []{ "едно","две","три","четири" },
                        MaxScore = 2,
                        Text = "Избери",
                        Id = id,
                        ImagePath = "",
                        AnswerIndexes = new bool[]{false, false, false, false}
                    }
                },
                QuestionOrder = new List<QuestionType>() { QuestionType.Open , QuestionType.Closed, QuestionType.Open}
            };
            Assert.AreEqual(JsonConvert.SerializeObject(real), JsonConvert.SerializeObject(test));
        }

        [Test]
        public async Task Edit_Correct()
        {
            var testDb = await testService.GetById(id);
            var test = testResultsService.ToEdit(testDb);
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
        public async Task IsTestTakenByStudentId_Correct()
        {
            Assert.IsTrue(await testService.IsTestTakenByStudentId(id, id));
        }
        
        [Test]
        public async Task TestsTakenByStudent_Correct()
        {
            var test = (await testService.TestsTakenByStudent(id, query)).Items.FirstOrDefault();
            var dbTest = GetTestByIdIncludingTestTaken(id);
            dbTest.IsTestTaken = true;
            string json1 = JsonConvert.SerializeObject(dbTest, Formatting.Indented,
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
        public async Task Create_Correct()
        {
            int testsCount = data.Tests.Count();
            var testModel = new TestViewModel()
            {
                CreatedOn = new DateTime(2023, 5, 19, 10, 51, 0),
                Description = "Test Test",
                Grade = 8,
                Time = 10,
                Title = "The test",
                Id = id2,
                PublicityLevel = PublicityLevel.Public,
                PhotoPath = "",
                QuestionOrder = "O|C|O"
            };
            await testService.Create(testModel, id, new string[0]);
            int testsCountNow = (await testService.GetAll(id, id, query)).Items.Count();
            Assert.AreEqual(testsCount+1, testsCountNow);
            SetUpBase();
            SetUp();
        }

        [Test]
        public async Task ExistsbyId_Correct()
        {
            Assert.IsTrue(await testService.ExistsbyId(id));
        }

        [Test]
        public async Task StudentHasAccess_Correct()
        {
            Assert.IsFalse(await testService.StudentHasAccess(id, id));
        }

        [Test]
        public async Task Delete_Correct()
        {
            await testService.DeleteTest(id);
            Assert.IsFalse(await testService.ExistsbyId(id));
            SetUpBase();
            SetUp();
        }

        [Test]
        public async Task Filter_Correct()
        {
            var teacher = await data.Teachers.FirstOrDefaultAsync(t => t.Id == id);
            var tests = new List<Test>()
            {
                new Test()
                {
                    CreatedOn = new DateTime(2023, 5, 19, 10, 51, 0),
                    Creator = teacher,
                    Description = "Test Test",
                    Grade = 10,
                    Time = 10,
                    Title = "Pesho",
                    PublicyLevel = PublicityLevel.TeachersOnly,
                    Id = id2,
                    PhotoPath = "",
                    Subject = Subject.Математика,
                    QuestionsOrder = "O|C|O"
                }
            };
            data.Tests.AddRange(tests);
            await data.SaveChangesAsync();
            SetUp();
            var testsDb = await data.Tests
                                    .Include(t => t.TestResults)
                                    .Include(t => t.TestLikes)
                                    .ToListAsync();
            query.Filters.Subject = Subject.Математика;
            var bySubject = await testService.Filter(data.Tests, query, null, null);
            Assert.AreEqual(id2, bySubject.Items.FirstOrDefault().Id);
            var bySubjectMine = await testService.FilterMine(testsDb, query);
            Assert.AreEqual(id2, bySubjectMine.Items.FirstOrDefault().Id);

            SetUpQuery();
            query.Filters.Grade = 10;
            var byGrade = await testService.Filter(data.Tests, query, null, null);
            Assert.AreEqual(id2, byGrade.Items.FirstOrDefault().Id);
            var byGradeMine = await testService.FilterMine(testsDb, query);
            Assert.AreEqual(id2, byGradeMine.Items.FirstOrDefault().Id);

            SetUpQuery();
            query.Filters.SearchTerm = "Pesho";
            var bySearchTerm = await testService.Filter(data.Tests, query, null, null);
            Assert.AreEqual(id2, bySearchTerm.Items.FirstOrDefault().Id);
            var bySearchTermMine = await testService.FilterMine(testsDb, query);
            Assert.AreEqual(id2, bySearchTermMine.Items.FirstOrDefault().Id);

            SetUpQuery();
            query.Filters.Sorting = Sorting.Examiners;
            var byExaminers = await testService.Filter(data.Tests, query, null, null);
            Assert.AreEqual(id, byExaminers.Items.FirstOrDefault().Id);
            var byExaminersMine = await testService.FilterMine(testsDb, query);
            Assert.AreEqual(id, byExaminersMine.Items.FirstOrDefault().Id);

            SetUpQuery();
            query.Filters.Sorting = Sorting.Score;
            var byScore = await testService.Filter(data.Tests, query, null, null);
            Assert.AreEqual(id, byScore.Items.FirstOrDefault().Id);
            var byScoreMine = await testService.FilterMine(testsDb, query);
            Assert.AreEqual(id, byScoreMine.Items.FirstOrDefault().Id);

            SetUpQuery();
            query.Filters.Sorting = Sorting.Questions;
            var byQuestions = await testService.Filter(data.Tests, query, null, null);
            Assert.AreEqual(id, byQuestions.Items.FirstOrDefault().Id);
            var byQuestionsMine = await testService.FilterMine(testsDb, query);
            Assert.AreEqual(id, byQuestionsMine.Items.FirstOrDefault().Id);

            SetUpQuery();
            SetUpBase();
            SetUp();
        }
    }
}
