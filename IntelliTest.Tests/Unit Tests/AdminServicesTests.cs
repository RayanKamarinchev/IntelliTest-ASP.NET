using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Lessons;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Services;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;
using NUnit.Framework;

namespace IntelliTest.Tests.Unit_Tests
{
    [TestFixture]
    public class AdminServicesTests : UnitTestBase
    {
        private ITestService testService;
        private IClassService classService;
        private ILessonService lessonService;
        private Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        private Guid privateItemsId = Guid.Parse("338571d0-b77b-41c9-bef2-35586aa16c64");

        [OneTimeSetUp]
        public void SetUp()
        {
            Test privateTest = new Test()
            {
                CreatedOn = new DateTime(2023, 7, 27),
                CreatorId = id,
                Grade = 8,
                Description = "None",
                PublicyLevel = PublicityLevel.Private,
                Time = 10,
                Title = "test",
                Id = privateItemsId,
                PhotoPath = ""
            };
            data.AddAsync(privateTest);

            Lesson privateLesson = new Lesson()
            {
                Content = "Hi",
                HtmlCotnent = "Hi",
                CreatorId = id,
                IsPrivate = true,
                Id = privateItemsId,
                Grade = 8,
                CreatedOn = new DateTime(2023, 7, 27),
                Title = "Lesson",
            };
            data.Lessons.Add(privateLesson);
            data.SaveChanges();

            testService = new TestService(data);
            classService = new ClassService(data);
            lessonService = new LessonService(data);
        }

        [Test]
        public async Task GetAllTestsAdmin_Correct()
        {
            var tests = await testService.GetAllAdmin(new QueryModel<TestViewModel>());
            Assert.IsTrue(tests.Items.Any(t=>t.Id == privateItemsId));
            Assert.AreEqual(data.Tests.Count(), tests.Items.Count());
        }
        [Test]
        public async Task GetAllLessonsAdmin_Correct()
        {
            var lessons = await lessonService.GetAllAdmin(new QueryModel<LessonViewModel>());
            Assert.IsTrue(lessons.Items.Any(t => t.Id == privateItemsId));
            Assert.AreEqual(data.Lessons.Count(), lessons.Items.Count());

        }
        [Test]
        public async Task GetAllClassesAdmin_Correct()
        {
            var classes = await classService.GetAllAdmin();
            Assert.AreEqual(data.Classes.Count(), classes.Count());
        }
    }
}
