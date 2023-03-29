using IntelliTest.Data;
using IntelliTest.Models.Tests;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

        public async Task<IEnumerable<TestViewModel>> GetMy(int teacherId)
        {
            return await context.Tests
                                .Where(t=>t.CreatorId== teacherId)
                                .Select(t=> new TestViewModel()
                                {
                                    AverageScore = t.AverageScore,
                                    ClosedQuestions = t.ClosedQuestions,
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


        public async Task Edit(int id, TestEditViewModel model, int teacherId)
        {
            var test = await context.Tests
                                    .Include(t=>t.OpenQuestions)
                                    .Include(t=>t.ClosedQuestions)
                                    .FirstOrDefaultAsync(t=>t.Id==id);
            List<OpenQuestion> openQuestions = model.OpenQuestions
                                                    .Select(q => new OpenQuestion()
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
            if (test.CreatorId==teacherId)
            {
                test.Title = model.Title;
                test.Description = model.Description;
                test.Grade = model.Grade;
                test.Time = model.Time;
                test.ClosedQuestions = closedQuestions;
                test.OpenQuestions = openQuestions;
                context.Update(test);
            }
            else
            {
                var newTest = new Test()
                {
                    Title = model.Title,
                    Description = model.Description,
                    Grade = model.Grade,
                    Time = model.Time,
                    ClosedQuestions = closedQuestions,
                    OpenQuestions = openQuestions,
                    CreatedOn = DateTime.Now,
                    CreatorId = teacherId
                };
                context.Tests.Add(newTest);
            }
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
                                           MaxScore = q.Question.MaxScore,
                                           Answer = q.Answer
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

        public TestStatsViewModel GetStatistics(int testId)
        {
            var studentIds = GetStudentIds(testId);
            List<TestReviewViewModel> res = new List<TestReviewViewModel>();
            foreach (var studentId in studentIds)
            {
                res.Add(TestResults(testId, studentId));
            }

            TestStatsViewModel model = new TestStatsViewModel();

            List<List<List<int>>> allClosedAnswers = new List<List<List<int>>>();
            res.ForEach(r =>
            {
                List<List<int>> answers = new List<List<int>>();
                r.ClosedQuestions.ForEach(q =>
                {
                    answers.Add(new List<int>());
                    for (int i = 0; i < q.Answers.Length; ++i)
                    {
                        if (q.Answers[i])
                        {
                            answers.Last().Add(i);
                        }
                    }
                });
                allClosedAnswers.Add(answers);
            });
            for (int i = 0; i < allClosedAnswers[0].Count; i++)
            {
                model.ClosedQuestions.Add(new ClosedQuestionStatsViewModel()
                {
                    StudentAnswers = allClosedAnswers.Select(a => a[i]).ToList(),
                    Text = res[0].ClosedQuestions[i].Text,
                    Answers = res[0].ClosedQuestions[i].PossibleAnswers,
                    Order = res[0].ClosedQuestions[i].Order
                });
            }

            List<List<string>> allOpenAnswers = new List<List<string>>();
            res.ForEach(r =>
            {
                List<string> answers = new List<string>();
                r.OpenQuestions.ForEach(q =>
                {
                    answers.Add(q.Answer);
                });
                allOpenAnswers.Add(answers);
            });
            for (int i = 0; i < allOpenAnswers[0].Count; i++)
            {
                model.OpenQuestions.Add(new OpenQuestionStatsViewModel()
                {
                    StudentAnswers = allOpenAnswers.Select(a => a[i]).ToList(),
                    Text = res[0].OpenQuestions[i].Text,
                    Order = res[0].OpenQuestions[i].Order
                });
            }


            return model;
        }

        public int[] GetStudentIds(int testId)
        {
            return context.OpenQuestionAnswers
                          .Where(q => q.Question.TestId == testId)
                          .Select(t => t.StudentId)
                          .Union(
                              context.ClosedQuestionAnswers
                                     .Where(q => q.Question.TestId == testId)
                                     .Select(t => t.StudentId)
                          ).ToArray();
        }

        public async Task<IEnumerable<TestViewModel>> TestsTakenByStudent(int studentId)
        {
            var student = await context.Students
                                       .Include(s=>s.ClosedAnswers)
                                       .ThenInclude(a=>a.Question)
                                       .ThenInclude(q=>q.Test)
                                       .Include(s=>s.OpenAnswers)
                                       .ThenInclude(a => a.Question)
                                       .ThenInclude(q => q.Test)
                                       .FirstOrDefaultAsync(s=>s.Id == studentId);
            var closedIds = student?.ClosedAnswers
                                 ?.Select(a => a.Question.Test)
                                 ?.Distinct() ?? new List<Test>();
            var openIds = student?.OpenAnswers
                                  ?.Select(a => a.Question.Test)
                                  ?.Distinct() ?? new List<Test>();
            return closedIds.Union(openIds)
                            .Select(t => new TestViewModel()
                            {
                                AverageScore = t.AverageScore,
                                ClosedQuestions = t.ClosedQuestions,
                                CreatedOn = t.CreatedOn,
                                Description = t.Description,
                                Grade = t.Grade,
                                Id = t.Id,
                                MaxScore = t.ClosedQuestions.Sum(q => q.MaxScore) + t.OpenQuestions.Sum(q => q.MaxScore),
                                OpenQuestions = t.OpenQuestions,
                                Time = t.Time,
                                Title = t.Title,
                                MultiSubmit = t.MultiSubmission
                            });

        }

        public async Task<int> Create(TestViewModel model, int teacherId)
        {
            Test test = new Test()
            {
                Title = model.Title,
                Description = model.Description,
                Subject = model.Subject,
                Time = model.Time,
                Grade = model.Grade,
                School = model.School,
                CreatedOn = DateTime.Now,
                CreatorId = teacherId,
                OpenQuestions = new List<OpenQuestion>(),
                ClosedQuestions = new List<ClosedQuestion>()
            };
            var e = await context.Tests.AddAsync(test);
            await context.SaveChangesAsync();
            return e.Entity.Id;
        }

        public Task<IEnumerable<TestResultsViewModel>> GetAllResults(int studentId)
        {
            var tests = context.OpenQuestionAnswers
                   .Where(q => q.StudentId == studentId)
                   .Select(q => q.Question.Test)
                   .Distinct()
                   .Union(context.ClosedQuestionAnswers
                                 .Where(q => q.StudentId == studentId)
                                 .Select(q => q.Question.Test)
                                 .Distinct());
            return tests.Select(t=>new TestResultsViewModel()
            {
                CreatedOn = t.CreatedOn,
                Grade = t.Grade,
                Description = t.Description,
                Title = t.Title,
                School = t.School,
                Score = t
            })
        }


        public async Task<bool> ExistsbyId(int id)
        {
            return await context.Tests.AnyAsync(t=>t.Id == id);
        }
    }
}
