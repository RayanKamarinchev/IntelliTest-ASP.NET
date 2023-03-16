using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Lessons;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntelliTest.Core.Services
{
    public class LessonService : ILessonService
    {
        private readonly IntelliTestDbContext context;

        public LessonService(IntelliTestDbContext _context)
        {
            context = _context;
        }
        public async Task<IEnumerable<LessonViewModel>> GetAll()
        {
            List<LessonViewModel> model = new List<LessonViewModel>();
            foreach (var l in context.Lessons
                                     .Include(l => l.ClosedQuestions)
                                     .Include(l => l.OpenQuestions))
            {
                model.Add(new LessonViewModel()
                {
                    ClosedQuestions = l.ClosedQuestions ?? new List<ClosedQuestion>(),
                    OpenQuestions = l.OpenQuestions ?? new List<OpenQuestion>(),
                    Content = l.Content,
                    CreatedOn = l.CreatedOn,
                    CreatorId = l.CreatorId,
                    Grade = l.Grade,
                    Id = l.Id,
                    Likes = l.Likes,
                    ReadingTime = l.ReadingTime,
                    Readers = l.Readers,
                    Title = l.Title,
                    School = l.School,
                    Subject = l.Subject
                });
            }

            return model;
        }

        public async Task<LessonViewModel> GetById(int lessonId)
        {
            var l = await context.Lessons
                                .Include(l => l.OpenQuestions)
                                .Include(l => l.ClosedQuestions)
                                .FirstOrDefaultAsync(l => l.Id == lessonId);
            return new LessonViewModel()
            {
                ClosedQuestions = l.ClosedQuestions ?? new List<ClosedQuestion>(),
                OpenQuestions = l.OpenQuestions ?? new List<OpenQuestion>(),
                Content = l.Content,
                CreatedOn = l.CreatedOn,
                CreatorId = l.CreatorId,
                Grade = l.Grade,
                Id = l.Id,
                Likes = l.Likes,
                ReadingTime = l.ReadingTime,
                Readers = l.Readers,
                Title = l.Title,
                School = l.School,
                Subject = l.Subject
            };
        }

        public EditLessonViewModel ToEdit(LessonViewModel model)
        {
            return new EditLessonViewModel()
            {
                Title = model.Title,
                ReadingTime = model.ReadingTime,
                School = model.School,
                ClosedQuestions = model.ClosedQuestions,
                OpenQuestions = model.OpenQuestions,
                Content = model.Content,
                Grade = model.Grade,
                Id = model.Id,
                Subject = model.Subject
            };
        }

        public async Task Create(EditLessonViewModel model, int teacherId)
        {
            Lesson lesson = new Lesson()
            {
                ClosedQuestions = model.ClosedQuestions,
                OpenQuestions = model.OpenQuestions,
                Content = model.Content,
                CreatedOn = DateTime.Now,
                CreatorId = teacherId,
                Grade = model.Grade,
                School = model.School,
                Subject = model.Subject,
                Title = model.Title,
                ReadingTime = model.Content.Split(" ").Length / 150
            };
            await context.Lessons.AddAsync(lesson);
            await context.SaveChangesAsync();
        }

        public Task<bool> ExistsById(int lessonId)
        {
            return context.Lessons.AnyAsync(l => l.Id == lessonId);
        }

        public async Task Edit(int lessonId, LessonViewModel model)
        {
            var lesson = await context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
            lesson.Grade = model.Grade;
            lesson.OpenQuestions = model.OpenQuestions;
            lesson.ClosedQuestions = model.ClosedQuestions;
            lesson.School = model.School;
            lesson.Content = model.Content;
            lesson.Title = model.Title;
            lesson.ReadingTime = model.Content.Split(" ").Length / 150;
            context.Update(lesson);
            await context.SaveChangesAsync();
        }
    }
}
