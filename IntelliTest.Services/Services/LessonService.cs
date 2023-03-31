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
                                     .Include(l=>l.LessonLikes)
                                     .Include(l => l.ClosedQuestions)
                                     .Include(l => l.OpenQuestions)
                                     .Include(l=>l.Reads)
                                     .Include(l=>l.Creator)
                                     .ThenInclude(c=>c.User))
            {
                var n = l.LessonLikes;
                int c = 0;
                if (n != null)
                {
                    c = n.Count();
                }
                model.Add(new LessonViewModel()
                {
                    ClosedQuestions = l.ClosedQuestions ?? new List<ClosedQuestion>(),
                    OpenQuestions = l.OpenQuestions ?? new List<OpenQuestion>(),
                    Content = l.Content,
                    CreatedOn = l.CreatedOn,
                    CreatorId = l.CreatorId,
                    Grade = l.Grade,
                    Id = l.Id,
                    Likes = c,
                    Readers = l.Reads.Count(),
                    Title = l.Title,
                    School = l.School,
                    Subject = l.Subject,
                    CreatorName = l.Creator.User.FirstName + l.Creator.User.LastName
                });
            }

            return model;
        }

        public async Task<LessonViewModel> GetById(Guid lessonId)
        {
            var l = await context.Lessons
                                .Include(l => l.OpenQuestions)
                                .Include(l => l.ClosedQuestions)
                                .Include(l => l.Reads)
                                .Include(l=>l.Creator)
                                .ThenInclude(t=>t.User)
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
                Likes = l.LessonLikes?.Count() ?? 0,
                Readers = l.Reads.Count(),
                Title = l.Title,
                School = l.School,
                Subject = l.Subject,
                CreatorName = l.Creator.User.FirstName + l.Creator.User.LastName,
                
            };
        }

        public EditLessonViewModel ToEdit(LessonViewModel model)
        {
            return new EditLessonViewModel()
            {
                Title = model.Title,
                School = model.School,
                ClosedQuestions = model.ClosedQuestions,
                OpenQuestions = model.OpenQuestions,
                Content = model.Content,
                Grade = model.Grade,
                Id = model.Id,
                Subject = model.Subject
            };
        }

        public async Task Create(EditLessonViewModel model, Guid teacherId)
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
                Title = model.Title
            };
            await context.Lessons.AddAsync(lesson);
            await context.SaveChangesAsync();
        }

        public async Task LikeLesson(Guid lessonId, string userId)
        {
            var lesson = await context.Lessons
                                .Include(l => l.LessonLikes)
                                .FirstOrDefaultAsync(l => l.Id == lessonId);
            lesson.LessonLikes.Add(new LessonLike()
            {
                UserId = userId
            });
            await context.SaveChangesAsync();
        }

        public async Task UnlikeLesson(Guid lessonId, string userId)
        {
            var lesson = await context.Lessons
                                      .Include(l => l.LessonLikes)
                                      .FirstOrDefaultAsync(l => l.Id == lessonId);
            lesson.LessonLikes.Remove(lesson.LessonLikes.Single(l => l.UserId == userId));
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsLiked(Guid lessonId, string userId)
        {
            var lesson = await context.Lessons
                                      .Include(l => l.LessonLikes)
                                      .FirstOrDefaultAsync(l => l.Id == lessonId);
            return lesson.LessonLikes.Any(l => l.UserId == userId);
        }

        public async Task Read(Guid lessonId, string userId)
        {
            bool exists = await context.Reads.AnyAsync(r => r.LessonId == lessonId && r.UserId == userId);
            if (!exists)
            {
                await context.Reads.AddAsync(new Read()
                {
                    LessonId = lessonId,
                    UserId = userId
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<LessonViewModel>> ReadLessons(string userId)
        {
            List<LessonViewModel> model = new List<LessonViewModel>();
            var lessons = await context.Lessons
                                 .Where(l => l.Reads.Any(r => r.UserId == userId))
                                 .Include(l => l.LessonLikes)
                                 .Include(l => l.ClosedQuestions)
                                 .Include(l => l.OpenQuestions)
                                 .Include(l => l.Reads)
                                 .Include(l => l.Creator)
                                 .ThenInclude(c => c.User)
                                 .ToListAsync();
            foreach (var l in lessons)
            {
                var n = l.LessonLikes;
                int c = 0;
                if (n != null)
                {
                    c = n.Count();
                }
                model.Add(new LessonViewModel()
                {
                    ClosedQuestions = l.ClosedQuestions ?? new List<ClosedQuestion>(),
                    OpenQuestions = l.OpenQuestions ?? new List<OpenQuestion>(),
                    Content = l.Content,
                    CreatedOn = l.CreatedOn,
                    CreatorId = l.CreatorId,
                    Grade = l.Grade,
                    Id = l.Id,
                    Likes = c,
                    Readers = l.Reads.Count(),
                    Title = l.Title,
                    School = l.School,
                    Subject = l.Subject,
                    CreatorName = l.Creator.User.FirstName + l.Creator.User.LastName
                });
            }

            return model;
        }

        public async Task<IEnumerable<LessonViewModel>> LikedLessons(string userId)
        {
            List<LessonViewModel> model = new List<LessonViewModel>();
            var lessons = await context.Lessons
                                       .Where(l => l.LessonLikes != null)
                                       .Where(l => l.LessonLikes.Any(l => l.UserId == userId))
                                       .Include(l => l.LessonLikes)
                                       .Include(l => l.ClosedQuestions)
                                       .Include(l => l.OpenQuestions)
                                       .Include(l => l.Reads)
                                       .Include(l => l.Creator)
                                       .ThenInclude(c => c.User)
                                       .ToListAsync();

            foreach (var l in lessons)
            {
                var n = l.LessonLikes;
                int c = 0;
                if (n != null)
                {
                    c = n.Count();
                }
                model.Add(new LessonViewModel()
                {
                    ClosedQuestions = l.ClosedQuestions ?? new List<ClosedQuestion>(),
                    OpenQuestions = l.OpenQuestions ?? new List<OpenQuestion>(),
                    Content = l.Content,
                    CreatedOn = l.CreatedOn,
                    CreatorId = l.CreatorId,
                    Grade = l.Grade,
                    Id = l.Id,
                    Likes = c,
                    Readers = l.Reads.Count(),
                    Title = l.Title,
                    School = l.School,
                    Subject = l.Subject,
                    CreatorName = l.Creator.User.FirstName + l.Creator.User.LastName
                });
            }

            return model;
        }

        public Task<bool> ExistsById(Guid lessonId)
        {
            return context.Lessons.AnyAsync(l => l.Id == lessonId);
        }

        public async Task Edit(Guid lessonId, LessonViewModel model)
        {
            var lesson = await context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
            lesson.Grade = model.Grade;
            lesson.OpenQuestions = model.OpenQuestions;
            lesson.ClosedQuestions = model.ClosedQuestions;
            lesson.School = model.School;
            lesson.Content = model.Content;
            lesson.Title = model.Title;
            context.Update(lesson);
            await context.SaveChangesAsync();
        }
    }
}
