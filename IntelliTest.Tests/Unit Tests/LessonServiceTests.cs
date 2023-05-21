using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Lessons;
using IntelliTest.Core.Services;
using IntelliTest.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using IntelliTest.Data.Enums;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NUnit.Framework.Constraints;
using OpenAI_API.Models;

namespace IntelliTest.Tests.Unit_Tests
{
    [TestFixture]
    public class LessonServiceTests : UnitTestBase
    {
        private ILessonService lessonService;
        private Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        private Guid id2 = Guid.Parse("fcda0a94-d7f6-4836-a093-f69066f177c7");
        [OneTimeSetUp]
        public void SetUp() =>
            lessonService = new LessonService(data);

        private QueryModel<LessonViewModel> query = new QueryModel<LessonViewModel>()
        {
            CurrentPage = 1,
            Filters = new Filter()
            {
                Sorting = Sorting.Likes,
                Subject = Subject.Няма
            },
            ItemsPerPage = 3
        };

        public void SetUpQuery() => query = new QueryModel<LessonViewModel>()
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
        public async Task Filter_Correct()
        {
            var teacher = await data.Teachers.FirstOrDefaultAsync(t => t.Id == id);
            var lessons = new List<Lesson>()
            {
                new Lesson()
                {
                    Content = "Example nonsense text but longer",
                    CreatedOn = new DateTime(2023, 5, 19, 11, 3, 0),
                    Creator = teacher,
                    Grade = 10,
                    HtmlCotnent = "Example nonsense text",
                    Title = "Bulgarian lesson",
                    Id = id2,
                    Subject = Subject.Български,
                }
            };
            data.Lessons.AddRange(lessons);
            await data.SaveChangesAsync();
            SetUp();
            var lessonsDb = data.Lessons
                                      .Include(l => l.LessonLikes)
                                      .Include(l => l.ClosedQuestions)
                                      .Include(l => l.OpenQuestions)
                                      .Include(l => l.Reads)
                                      .Include(l => l.Creator)
                                      .ThenInclude(c => c.User);
            query.Filters.Subject = Subject.Математика;
            var bySubject = await lessonService.Filter(lessonsDb, query);
            Assert.AreEqual(id, bySubject.Items.FirstOrDefault().Id);

            SetUpQuery();
            query.Filters.Grade = 10;
            var byGrade = await lessonService.Filter(lessonsDb, query);
            Assert.AreEqual(id2, byGrade.Items.FirstOrDefault().Id);

            SetUpQuery();
            query.Filters.SearchTerm = "Bul";
            var bySearchTerm = await lessonService.Filter(lessonsDb, query);
            Assert.AreEqual(id2, bySearchTerm.Items.FirstOrDefault().Id);

            SetUpQuery();
            query.Filters.Sorting = Sorting.Readers;
            var byExaminers = await lessonService.Filter(lessonsDb, query);
            Assert.AreEqual(id, byExaminers.Items.FirstOrDefault().Id);

            SetUpQuery();
            query.Filters.Sorting = Sorting.ReadingTime;
            var byScore = await lessonService.Filter(lessonsDb, query);
            Assert.AreEqual(id, byScore.Items.FirstOrDefault().Id);

            SetUpQuery();
            SetUpBase();
            SetUp();
        }

        [Test]
        public async Task GetById_Correct()
        {
            var lesson = await lessonService.GetById(id);
            var real = await data.Lessons.FirstOrDefaultAsync(t => t.Id == id);
            Assert.AreEqual(real.Id, lesson.Id);
        }

        [Test]
        public async Task GetAll_Correct()
        {
            var lessonsCount = (await lessonService.GetAll(null, query)).Items.Count();
            var real = data.Lessons.Where(l=>!l.IsDeleted).Count();
            Assert.AreEqual(lessonsCount, real);
        }

        [Test]
        public async Task GetByName_Correct()
        {
            var named = await lessonService.GetByName("Math lesson");
            Assert.AreEqual(JsonConvert.SerializeObject(named), JsonConvert.SerializeObject(await lessonService.GetById(named.Id)));
        }
        [Test]
        public async Task ToEdit_Correct()
        {
            var lesson = await lessonService.GetById(id);
            var editLesson = lessonService.ToEdit(lesson);
            var real = new EditLessonViewModel()
            {
                Title = lesson.Title,
                School = lesson.School,
                ClosedQuestions = lesson.ClosedQuestions,
                OpenQuestions = lesson.OpenQuestions,
                Content = lesson.Content,
                Grade = lesson.Grade,
                Id = lesson.Id,
                Subject = lesson.Subject,
                HtmlContent = lesson.HtmlContent
            };
            Assert.AreEqual(JsonConvert.SerializeObject(editLesson), JsonConvert.SerializeObject(real));
        }

        [Test]
        public async Task Create_Correct()
        {
            int lessonsCount = data.Lessons.Count();
            var lesson = await data.Lessons
                                   .Include(l=>l.Creator)
                                   .FirstOrDefaultAsync(l=>l.Id == id);
            var real = new EditLessonViewModel()
            {
                Title = lesson.Title,
                School = lesson.Creator.School,
                ClosedQuestions = lesson.ClosedQuestions,
                OpenQuestions = lesson.OpenQuestions,
                Content = lesson.Content,
                Grade = lesson.Grade,
                Id = lesson.Id,
                Subject = lesson.Subject,
                HtmlContent = lesson.HtmlCotnent
            };
            await lessonService.Create(real, id);
            int lessonsCountNow = data.Lessons.Count();
            Assert.AreEqual(lessonsCount + 1, lessonsCountNow);
            SetUpBase();
            SetUp();
        }

        [Test]
        public async Task Like_Correct()
        {
            var lesson = await data.Lessons
                                   .Include(l => l.LessonLikes)
                                   .FirstOrDefaultAsync(l => l.Id == id);
            int lessonsLikesCount = lesson.LessonLikes.Count;
            await lessonService.LikeLesson(id, "StudentUser");
            int lessonLikesCountNow = lesson.LessonLikes.Count;
            Assert.AreEqual(lessonsLikesCount + 1, lessonLikesCountNow);
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task Unlike_Correct()
        {
            var lesson = await data.Lessons
                                   .Include(l => l.LessonLikes)
                                   .FirstOrDefaultAsync(l => l.Id == id);
            lesson.LessonLikes.Add(new LessonLike()
            {
                UserId = "StudentUser"
            });
            int lessonsLikesCount = lesson.LessonLikes.Count;
            await lessonService.UnlikeLesson(id, "StudentUser");
            int lessonLikesCountNow = lesson.LessonLikes.Count;
            Assert.AreEqual(lessonsLikesCount - 1, lessonLikesCountNow);
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task IsLiked_Correct()
        {
            var lesson = await data.Lessons
                                   .Include(l => l.LessonLikes)
                                   .FirstOrDefaultAsync(l => l.Id == id);
            lesson.LessonLikes.Add(new LessonLike()
            {
                UserId = "StudentUser"
            });
            Assert.IsTrue(await lessonService.IsLiked(id, "StudentUser"));
            SetUpBase();
            SetUp();
        }

        [Test]
        public async Task Read_Correct()
        {
            var lesson = await data.Lessons
                                   .Include(l => l.Reads)
                                   .FirstOrDefaultAsync(l => l.Id == id);
            int lessonsReadsCount = lesson.Reads.Count();
            await lessonService.Read(id, "StudentUser");
            int lessonReadsCountNow = lesson.Reads.Count();
            Assert.AreEqual(lessonsReadsCount + 1, lessonReadsCountNow);
            SetUpBase();
            SetUp();
        }

        [Test]
        public async Task ReadLessons_Correct()
        {
            await lessonService.Read(id, "StudentUser");
            var readLesson = (await lessonService.ReadLessons("StudentUser")).FirstOrDefault();
            Assert.AreEqual(JsonConvert.SerializeObject(await lessonService.GetById(id)), JsonConvert.SerializeObject(readLesson));
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task LikedLessons_Correct()
        {
            await lessonService.LikeLesson(id, "StudentUser");
            var likedLesson = (await lessonService.LikedLessons("StudentUser")).FirstOrDefault();
            Assert.AreEqual(JsonConvert.SerializeObject(await lessonService.GetById(id)), JsonConvert.SerializeObject(likedLesson));
            SetUpBase();
            SetUp();
        }

    }
}
