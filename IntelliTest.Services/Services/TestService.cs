using IntelliTest.Data;
using IntelliTest.Models.Tests;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntelliTest.Core.Services
{
    public class TestService : ITestService
    {
        private readonly IntelliTestDbContext context;

        public TestService(IntelliTestDbContext _context)
        {
            context = _context;
        }

        public async Task<IEnumerable<TestViewModel>> GetAll()
        {
            return await context.Tests.Where(t=>!t.IsDeleted)
                                .Select(t=> new TestViewModel()
                                {
                                    AverageScore = t.AverageScore,
                                    ClosedQuestions = t.ClosedQuestions,
                                    Color1 = t.Color1,
                                    Color2 = t.Color2,
                                    CreatedOn = t.CreatedOn,
                                    Description = t.Description,
                                    Grade = t.Grade,
                                    Id = t.Id,
                                    MaxScore = t.MaxScore,
                                    OpenQuestions = t.OpenQuestions,
                                    Time = t.Time,
                                    Title = t.Title
                                })
                           .ToListAsync();
        }

        public async Task<IEnumerable<TestViewModel>> GetMy(string userId)
        {
            return await context.Tests
                                .Select(t=> new TestViewModel()
                                {
                                    AverageScore = t.AverageScore,
                                    ClosedQuestions = t.ClosedQuestions,
                                    Color1 = t.Color1,
                                    Color2 = t.Color2,
                                    CreatedOn = t.CreatedOn,
                                    Description = t.Description,
                                    Grade = t.Grade,
                                    Id = t.Id,
                                    MaxScore = t.MaxScore,
                                    OpenQuestions = t.OpenQuestions,
                                    Time = t.Time,
                                    Title = t.Title
                                })
                                .ToListAsync();
        }

        public async Task<TestViewModel> GetById(int id)
        {
            var t = await context.Tests
                                 .Include(t=>t.OpenQuestions)
                                 .Include(t=>t.ClosedQuestions)
                                 .FirstOrDefaultAsync(t=>t.Id == id);
            return new TestViewModel()
            {
                AverageScore = t.AverageScore,
                ClosedQuestions = t.ClosedQuestions,
                Color1 = t.Color1,
                Color2 = t.Color2,
                CreatedOn = t.CreatedOn,
                Description = t.Description,
                Grade = t.Grade,
                Id = t.Id,
                MaxScore = t.MaxScore,
                OpenQuestions = t.OpenQuestions,
                Time = t.Time,
                Title = t.Title
            };
        }

        public TestEditViewModel ToEdit(TestViewModel model)
        {
            var t = new TestEditViewModel()
            {
                OpenQuestions = model.OpenQuestions
                                     .Where(q => !q.IsDeleted)
                                     .Select(q => new OpenQuestionViewModel()
                                     {
                                         Answer = q.Answer,
                                         IsDeleted = false,
                                         Order = q.Order,
                                         Text = q.Text
                                     })
                                     .ToList(),
                ClosedQuestions = model.ClosedQuestions
                                       .Where(q => !q.IsDeleted)
                                       .Select(q => new ClosedQuestionViewModel()
                                       {
                                           Answers = q.Answers.Split("&"),
                                           AnswerIndexes = ProccessAnswerIndexes(q.Answers.Split("&"), q.AnswerIndexes),
                                           IsDeleted = false,
                                           Order = q.Order,
                                           Text = q.Text
                                       })
                                       .ToList(),
                Time = model.Time,
                Description = model.Description,
                Grade = model.Grade,
                Title = model.Title
            };
            return t;
        }

        private bool[] ProccessAnswerIndexes(string[] answers, string answerIndexes)
        {
            var list = Enumerable.Repeat(false, answers.Length).ToArray();
            var listOfIndx = answerIndexes.Split("&").Select(int.Parse);
            for (int i = 0; i < list.Length; i++)
            {
                if (listOfIndx.Contains(i))
                {
                    list[i] = true;
                }
            }

            return list;
        }

        public bool ExistsbyId(int id)
        {
            var all = context.Tests.ToList();
            return all.Any(t=>t.Id == id);
        }
    }
}
