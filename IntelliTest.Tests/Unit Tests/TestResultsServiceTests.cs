using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Questions.Closed;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Services;
using IntelliTest.Data.Enums;
using NUnit.Framework;

namespace IntelliTest.Tests.Unit_Tests
{
    [TestFixture]
    public class TestResultsServiceTests : UnitTestBase
    {
        private ITestResultsService testResultsService;
        private ITestService testService;
        private Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        private Guid id2 = Guid.Parse("fcda0a94-d7f6-4836-a093-f69066f177c7");

        [OneTimeSetUp]
        public void SetUp()
        {
            testResultsService = new TestResultsService(data, configuration);
            testService = new TestService(data);
        }

        [Test]
        public async Task ToEdit_Correct()
        {
            var testDb = await testService.GetById(id);
            var test = testResultsService.ToEdit(testDb);
            var real = new GroupEditViewModel()
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
        public async Task CalculateClosedQuestionScore_Correct()
        {
            Assert.AreEqual(1.5, testResultsService.CalculateClosedQuestionScore(new[] { false, true, false, false }, new[] { 1, 3 }, 3));
        }

        [Test]
        public async Task AddTestAnswer_Correct()
        {
            var OpenQuestions = new List<OpenQuestionViewModel>()
            {
                new OpenQuestionViewModel()
                {
                    MaxScore = 3,
                    Id = id,
                    Text = "Who are you",
                    Answer = "Its me, Mario"
                },
                new OpenQuestionViewModel()
                {
                    Text = "How are you",
                    MaxScore = 1,
                    Id = id2,
                    Answer = "Fine"
                }
            };
            var ClosedQuestions = new List<ClosedQuestionViewModel>()
            {
                new ClosedQuestionViewModel()
                {
                    Answers = new[] { "едно", "две", "три", "четири" },
                    MaxScore = 2,
                    Text = "Избери",
                    Id = id,
                    AnswerIndexes = new []{false, true, false, true}
                }
            };
            data.ClosedQuestionAnswers.RemoveRange(data.ClosedQuestionAnswers);
            data.OpenQuestionAnswers.RemoveRange(data.OpenQuestionAnswers);
            data.TestResults.RemoveRange(data.TestResults);
            await data.SaveChangesAsync();
            await testResultsService.SaveStudentTestAnswer(OpenQuestions, ClosedQuestions, id, id);
            Assert.AreEqual(2, data.OpenQuestionAnswers.Count());
            Assert.AreEqual(1, data.ClosedQuestionAnswers.Count());
            SetUpBase();
            SetUp();
        }

        [Test]
        public async Task ProccessAnswerIndexes_Correct()
        {
            Assert.AreEqual(new[] { false, true, false, true }, testResultsService.ProccessAnswerIndexes(new[] { "one", "two", "three", "four" }, "1&3"));
        }
        [Test]
        public void GetExaminersIds_Correct()
        {
            Assert.AreEqual(id, testResultsService.GetExaminersIds(id)[0]);
        }
        [Test]
        public async Task GetStatistics_Correct()
        {
            var res = await testResultsService.GetStatistics(id);
            Assert.AreEqual("The test", res.Title);
            Assert.AreEqual(1, res.TestGroups[0].ClosedQuestions.Count);
            Assert.AreEqual(2, res.TestGroups[0].OpenQuestions.Count);
            Assert.AreEqual(1, res.Examiners);
            Assert.AreEqual(2, res.TestGroups[0].ClosedQuestions.FirstOrDefault().StudentAnswers[0][0]);
            Assert.AreEqual("its me mario", res.TestGroups[0].OpenQuestions.FirstOrDefault().StudentAnswers[0]);
            Assert.AreEqual("Bad", res.TestGroups[0].OpenQuestions.ToList()[1].StudentAnswers[0]);
        }
    }
}
