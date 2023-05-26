using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Lessons;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;
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

        private Func<Lesson, int, LessonViewModel> ToViewModel = (l, c) => new LessonViewModel()
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
            School = l.Creator.School,
            Subject = l.Subject,
            CreatorName = l.Creator.User.FirstName + l.Creator.User.LastName,
            HtmlContent = l.HtmlCotnent
        };

        public async Task<QueryModel<LessonViewModel>> Filter(IQueryable<Lesson> lessonQuery,
                                                              QueryModel<LessonViewModel> query)
        {
            if (query.Filters.Subject != Subject.Няма)
            {
                lessonQuery = lessonQuery.Where(l => l.Subject == query.Filters.Subject);
            }

            if (query.Filters.Grade >= 1 && query.Filters.Grade <= 12)
            {
                lessonQuery = lessonQuery.Where(t => t.Grade == query.Filters.Grade);
            }

            if (string.IsNullOrEmpty(query.Filters.SearchTerm) == false)
            {
                query.Filters.SearchTerm = $"%{query.Filters.SearchTerm.ToLower()}%";

                lessonQuery = lessonQuery
                    .Where(l => EF.Functions.Like(l.Title.ToLower(), query.Filters.SearchTerm) ||
                                EF.Functions.Like(l.Creator.School.ToLower(), query.Filters.SearchTerm) ||
                                EF.Functions.Like(l.Content.ToLower(), query.Filters.SearchTerm));
            }

            if (query.Filters.Sorting == Sorting.Likes)
            {
                lessonQuery = lessonQuery.OrderBy(l => l.LessonLikes.Count());
            }
            else if (query.Filters.Sorting == Sorting.Readers)
            {
                lessonQuery = lessonQuery.OrderBy(l => l.Reads.Count());
            }
            else if (query.Filters.Sorting == Sorting.ReadingTime)
            {
                lessonQuery = lessonQuery.OrderBy(l => l.Content.Split().Length);
            }

            var lessonsDb = await lessonQuery.Skip(query.ItemsPerPage * (query.CurrentPage - 1))
                                       .Take(query.ItemsPerPage)
                                       .Include(l=>l.Creator)
                                       .Include(l=>l.Reads)
                                       .Include(l=>l.OpenQuestions)
                                       .Include(l=>l.ClosedQuestions)
                                       .ToListAsync();
            var lessons = new List<LessonViewModel>();
            foreach (var l in lessonsDb)
            {
                var n = l.LessonLikes;
                int c = 0;
                if (n != null)
                {
                    c = n.Count();
                }

                lessons.Add(ToViewModel(l,c));
            }

            query.Items = lessons;
            query.TotalItemsCount = lessons.Count;
            return query;
        }

        public async Task<QueryModel<LessonViewModel>> GetAll(Guid? teacherId, QueryModel<LessonViewModel> query)
        {
            var lessonsQuery = context.Lessons
                                      .Where(l => !l.IsDeleted && (!l.IsPrivate || l.CreatorId == teacherId))
                                      .Include(l => l.LessonLikes)
                                      .Include(l => l.ClosedQuestions)
                                      .Include(l => l.OpenQuestions)
                                      .Include(l => l.Reads)
                                      .Include(l => l.Creator)
                                      .ThenInclude(c => c.User)
                                      .Where(l => !l.IsPrivate);
            return await Filter(lessonsQuery, query);
        }

        public async Task<LessonViewModel?>? GetById(Guid lessonId)
        {
            var l = await context.Lessons
                                 .Where(l => !l.IsDeleted)
                                 .Include(l => l.LessonLikes)
                                 .Include(l => l.ClosedQuestions)
                                 .Include(l => l.OpenQuestions)
                                 .Include(l => l.Reads)
                                 .Include(l => l.Creator)
                                 .ThenInclude(c => c.User)
                                 .FirstOrDefaultAsync(l => l.Id == lessonId);
            if (l == null)
            {
                return null;
            }

            return ToViewModel(l, l.LessonLikes?.Count() ?? 0);
        }

        public async Task<LessonViewModel?>? GetByName(string name)
        {
            var l = await context.Lessons
                                 .Where(l => !l.IsDeleted)
                                 .Include(l => l.OpenQuestions)
                                 .Include(l => l.ClosedQuestions)
                                 .Include(l => l.Reads)
                                 .Include(l => l.Creator)
                                 .ThenInclude(t => t.User)
                                 .FirstOrDefaultAsync(l => l.Title == name);
            if (l == null)
            {
                return null;
            }

            return ToViewModel(l, l.LessonLikes?.Count() ?? 0);
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
                Subject = model.Subject,
                HtmlContent = model.HtmlContent
            };
        }

        public async Task Create(EditLessonViewModel model, Guid teacherId)
        {
            Lesson lesson = new Lesson()
            {
                ClosedQuestions = model.ClosedQuestions,
                OpenQuestions = model.OpenQuestions,
                Content = model.Content,
                HtmlCotnent = model.HtmlContent,
                CreatedOn = DateTime.Now,
                CreatorId = teacherId,
                Grade = model.Grade,
                Subject = model.Subject,
                Title = model.Title,
            };
            await context.Lessons.AddAsync(lesson);
            await context.SaveChangesAsync();
        }

        public async Task LikeLesson(Guid lessonId, string userId)
        {
            var lesson = await context.Lessons
                                      .Where(l => !l.IsDeleted)
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
                                      .Where(l=>!l.IsDeleted)
                                      .Include(l => l.LessonLikes)
                                      .FirstOrDefaultAsync(l => l.Id == lessonId);
            lesson.LessonLikes.Remove(lesson.LessonLikes.Single(l => l.UserId == userId));
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsLiked(Guid lessonId, string userId)
        {
            var lesson = await context.Lessons
                                      .Where(l=>!l.IsDeleted)
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
                                       .Where(l => l.Reads.Any(r => r.UserId == userId) && !l.IsDeleted)
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

                model.Add(ToViewModel(l, c));
            }

            return model;
        }

        public async Task<IEnumerable<LessonViewModel>> LikedLessons(string userId)
        {
            List<LessonViewModel> model = new List<LessonViewModel>();
            var lessons = await context.Lessons
                                       .Where(l => l.LessonLikes != null)
                                       .Where(l => l.LessonLikes.Any(l => l.UserId == userId))
                                       .Where(l=>!l.IsDeleted)
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

                model.Add(ToViewModel(l, c));
            }

            return model;
        }

        public Task<bool> ExistsById(Guid teacherId,Guid lessonId)
        {
            return context.Lessons
                          .Where(l => !l.IsDeleted && (!l.IsPrivate || l.CreatorId == teacherId))
                          .AnyAsync(l => l.Id == lessonId);
        }

        public async Task Edit(Guid lessonId, EditLessonViewModel model)
        {
            var lesson = await context.Lessons
                                      .Where(l => !l.IsDeleted)
                                      .FirstOrDefaultAsync(l => l.Id == lessonId);
            lesson.Grade = model.Grade;
            lesson.OpenQuestions = model.OpenQuestions;
            lesson.ClosedQuestions = model.ClosedQuestions;
            lesson.Content = model.Content;
            lesson.Title = model.Title;
            lesson.HtmlCotnent = model.HtmlContent;
            context.Update(lesson);
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsLessonCreator(Guid lessonId, Guid teacherId)
        {
            var teacher = await context.Teachers
                                       .Include(t => t.Lessons)
                                       .FirstOrDefaultAsync(t => t.Id == teacherId);
            if (teacher is null)
            {
                return false;
            }
            return teacher.Lessons.Any(t => t.Id == lessonId);
        }
    }
}