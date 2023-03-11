using IntelliTest.Data;
using IntelliTest.Models.Tests;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Questions;
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
                                    MaxScore = t.ClosedQuestions.Sum(q => q.MaxScore) + t.OpenQuestions.Sum(q => q.MaxScore),
                                    OpenQuestions = t.OpenQuestions,
                                    Time = t.Time,
                                    Title = t.Title,
                                    MultiSubmit = t.MultiSubmission
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
                                    MaxScore = t.ClosedQuestions.Sum(q=>q.MaxScore) + t.OpenQuestions.Sum(q=>q.MaxScore),
                                    OpenQuestions = t.OpenQuestions,
                                    Time = t.Time,
                                    Title = t.Title,
                                    MultiSubmit = t.MultiSubmission
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
                MaxScore = t.ClosedQuestions.Sum(q => q.MaxScore) + t.OpenQuestions.Sum(q => q.MaxScore),
                OpenQuestions = t.OpenQuestions,
                Time = t.Time,
                Title = t.Title,
                MultiSubmit = t.MultiSubmission
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
                                         Text = q.Text,
                                         MaxScore = q.MaxScore
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
                                           Text = q.Text,
                                           MaxScore = q.MaxScore
                                       })
                                       .ToList(),
                Time = model.Time,
                Description = model.Description,
                Grade = model.Grade,
                Title = model.Title,
                
            };
            return t;
        }
        public TestSubmitViewModel ToSubmit(TestViewModel model)
        {
            var t = new TestSubmitViewModel()
            {
                OpenQuestions = model.OpenQuestions
                                     .Where(q => !q.IsDeleted)
                                     .Select(q => new OpenQuestionAnswerViewModel()
                                     {
                                         Order = q.Order,
                                         Text = q.Text,
                                         Id = q.Id,
                                         MaxScore = q.MaxScore
                                     })
                                     .ToList(),
                ClosedQuestions = model.ClosedQuestions
                                       .Where(q => !q.IsDeleted)
                                       .Select(q => new ClosedQuestionAnswerViewModel()
                                       {
                                           PossibleAnswers = q.Answers.Split("&"),
                                           IsDeleted = false,
                                           Order = q.Order,
                                           Text = q.Text,
                                           Id = q.Id,
                                           MaxScore = q.MaxScore
                                       })
                                       .ToList(),
                Time = model.Time,
                Title = model.Title
            };
            return t;
        }


        public async Task Edit(int id, TestEditViewModel model)
        {
            var test = await context.Tests
                                    .Include(t=>t.OpenQuestions)
                                    .Include(t=>t.ClosedQuestions)
                                    .FirstOrDefaultAsync(t=>t.Id==id);
            test.Title = model.Title;
            test.Description = model.Description;
            test.Grade = model.Grade;
            test.Time = model.Time;
            List<OpenQuestion> openQuestions = model.OpenQuestions
                                                    .Select(q=>new OpenQuestion()
                                                    {
                                                        Text = q.Text,
                                                        Answer = q.Answer,
                                                        Order = q.Order,
                                                        MaxScore = q.MaxScore
                                                    }).ToList();
            List<ClosedQuestion> closedQuestions = model.ClosedQuestions
                                                    .Select(q => new ClosedQuestion()
                                                    {
                                                        Text = q.Text,
                                                        AnswerIndexes = string.Join("&", q.AnswerIndexes
                                                                                     .Select((val, indx) => new { val, indx })
                                                                                     .Where(q => q.val)
                                                                                     .Select(q => q.indx)),
                                                        Answers = string.Join("&", q.Answers),
                                                        Order = q.Order,
                                                        MaxScore = q.MaxScore
                                                    }).ToList();
            test.ClosedQuestions = closedQuestions;
            test.OpenQuestions = openQuestions;
            context.Update(test);
            await context.SaveChangesAsync();
        }

        public TestReviewViewModel TestResults(int testId, int studentId)
        {
            var openQuestions = context.OpenQuestionAnswers
                                       .Where(q => q.StudentId == studentId && q.Question.TestId == testId)
                                       .Include(q => q.Question)
                                       .Select(q => new OpenQuestionReviewViewModel()
                                       {
                                           Order = q.Question.Order,
                                           Text = q.Question.Text,
                                           Id = q.Id,
                                           RightAnswer = q.Question.Answer,
                                           MaxScore = q.Question.MaxScore
                                       })
                                       .ToList();
            var closedQuestions = new List<ClosedQuestionReviewViewModel>();
            var db = context.ClosedQuestionAnswers
                            .Where(q => q.StudentId == studentId && q.Question.TestId == testId)
                            .Include(q => q.Question);
            foreach (var q in db)
            {
                closedQuestions.Add(new ClosedQuestionReviewViewModel()
                {
                    PossibleAnswers = q.Question.Answers.Split("&", System.StringSplitOptions.None),
                    IsDeleted = false,
                    Order = q.Question.Order,
                    Text = q.Question.Text,
                    Id = q.Id,
                    Answers = ProccessAnswerIndexes(q.Question.Answers.Split("&", System.StringSplitOptions.None), q.AnswerIndexes),
                    RightAnswers = q.Question.AnswerIndexes.Split("&", System.StringSplitOptions.None).Select(int.Parse).ToArray(),
                    MaxScore = q.Question.MaxScore
                });
            }

            return new TestReviewViewModel()
            {
                OpenQuestions = openQuestions,
                ClosedQuestions = closedQuestions
            };
        }
        public static bool[] ProccessAnswerIndexes(string[] answers, string answerIndexes)
        {
            var list = Enumerable.Repeat(false, answers.Length).ToArray();
            if (answerIndexes=="")
            {
                return list;
            }
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

        public async Task<bool> IsTestTakenByStudentId(int testId, Student student)
        {
            bool closed = (student?.ClosedAnswers?.Any(a => a?.Question?.Test?.Id == testId) ?? false);
            bool open = (student?.OpenAnswers?.Any(a => a?.Question?.Test?.Id == testId) ?? false);
            return closed || open;
        }

        public async Task<bool> ExistsbyId(int id)
        {
            return await context.Tests.AnyAsync(t=>t.Id == id);
        }
    }
}
