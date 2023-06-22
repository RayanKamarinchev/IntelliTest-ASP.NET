using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IntelliTest.Core.Services
{
    public class TestResultsService : ITestResultsService
    {
        private readonly IntelliTestDbContext context;
        private readonly IConfiguration config;
        private readonly TestResultsProcessorService testResultsProcessor;

        public TestResultsService(IntelliTestDbContext _context, IConfiguration _config)
        {
            context = _context;
            config = _config;
            if (!(string.IsNullOrEmpty(config["ConnectionString"]) || string.IsNullOrEmpty(config["OpenAIKey"])))
            {
                testResultsProcessor = new TestResultsProcessorService(config["OpenAIKey"], config["ConnectionString"], _context);
            }
        }

        private Func<TestResult, TestResultsViewModel> ToResultsViewModel = t => new TestResultsViewModel()
        {
            TakenOn = t.TakenOn,
            Title = t.Test.Title,
            Description = t.Test.Description,
            Grade = t.Test.Grade,
            Mark = t.Mark,
            Score = t.Score,
            TestId = t.TestId
        };


        public async Task AddTestAnswer(List<OpenQuestionAnswerViewModel> openQuestions,
                                        List<ClosedQuestionAnswerViewModel> closedQuestions, Guid studentId, Guid testId)
        {
            if (context.TestResults.Any(t => t.StudentId == studentId && t.TestId == testId))
            {
                return;
            }
            var open = openQuestions?.Select(q => new OpenQuestionAnswer()
            {
                Answer = q.Answer,
                QuestionId = q.Id,
                StudentId = studentId
            });
            var closed = closedQuestions?.Select(q => new ClosedQuestionAnswer()
            {
                AnswerIndexes = string.Join("&", q.Answers
                                                  .Select((val, indx) => new { val, indx })
                                                  .Where(q => q.val)
                                                  .Select(q => q.indx)),
                QuestionId = q.Id,
                StudentId = studentId
            });

            if (open != null)
            {
                await context.OpenQuestionAnswers.AddRangeAsync(open);
            }
            if (closed != null)
            {
                await context.ClosedQuestionAnswers.AddRangeAsync(closed);
            }
            await context.SaveChangesAsync();

            await context.TestResults.AddAsync(new TestResult()
            {
                Mark = Mark.Unmarked,
                Score = 0,
                StudentId = studentId,
                TestId = testId,
                TakenOn = DateTime.Now
            });
            await context.SaveChangesAsync();
        }

        public decimal CalculateClosedQuestionScore(bool[] Answers, int[] RightAnswers, int MaxScore)
        {
            decimal score = 0;
            for (int i = 0; i < Answers.Length; i++)
            {
                if (Answers[i])
                {
                    if (RightAnswers.Contains(i))
                    {
                        score++;
                    }
                    else
                    {
                        score--;
                    }
                }
            }
            score *= MaxScore * 1.0m / RightAnswers.Count();
            if (score < 0)
            {
                score = 0;
            }

            return score;
        }

        public bool[] ProccessAnswerIndexes(string[] answers, string answerIndexes)
        {
            var list = Enumerable.Repeat(false, answers.Length).ToArray();
            if (answerIndexes == "")
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
                Id = model.Id,
                PublicityLevel = model.PublicityLevel
            };
            return t;
        }


        public Guid[] GetExaminersIds(Guid testId)
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

        public async Task<TestStatsViewModel> GetStatistics(Guid testId)
        {
            var studentIds = GetExaminersIds(testId);
            List<TestReviewViewModel> res = new List<TestReviewViewModel>();
            foreach (var studentId in studentIds)
            {
                res.Add(await GetAllTestResults(testId, studentId));
            }

            TestStatsViewModel model = new TestStatsViewModel();
            Test test = await context.Tests
                                     .Include(t=>t.TestResults)
                                     .FirstOrDefaultAsync(t=>t.Id == testId);
            model.AverageScore = Math.Round(!test.TestResults.Any() ? 0 : test.TestResults.Average(r => r.Score), 2);
            model.Title = test.Title;
            model.Examiners = test.TestResults.Count();

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
            if (allClosedAnswers.Count > 0)
            {
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
            if (allOpenAnswers.Count > 0)
            {
                for (int i = 0; i < allOpenAnswers[0].Count; i++)
                {
                    model.OpenQuestions.Add(new OpenQuestionStatsViewModel()
                    {
                        StudentAnswers = allOpenAnswers.Select(a => a[i]).ToList(),
                        Text = res[0].OpenQuestions[i].Text,
                        Order = res[0].OpenQuestions[i].Order
                    });
                }
            }


            return model;
        }

        public async Task<List<TestStatsViewModel>> TestsTakenByClass(Guid classId)
        {
            Class? classDb = await context.Classes
                                          .Include(c => c.ClassTests)
                                          .FirstOrDefaultAsync(c => c.Id == classId);
            if (classDb == null)
            {
                return null;
            }

            var res = classDb.ClassTests.Select(async (ct) => await GetStatistics(ct.TestId));
            List<TestStatsViewModel> model = new List<TestStatsViewModel>();
            foreach (var test in res)
            {
                model.Add(await test);
            }

            return model;
        }

        public async Task<TestResultsViewModel> GetTestResult(Guid testId, Guid studentId)
        {
            TestResult testResult = await context
                         .TestResults
                         .FirstOrDefaultAsync(t => t.TestId == testId && t.StudentId == studentId);
            return ToResultsViewModel(testResult);
        }

        public async Task<TestReviewViewModel> GetAllTestResults(Guid testId, Guid studentId)
        {

            var openQuestionAnswers = await context.OpenQuestionAnswers
                                                   .Include(q => q.Question)
                                                   .Where(q => q.StudentId == studentId
                                                            && q.Question.TestId == testId
                                                            && string.IsNullOrEmpty(q.Explanation))
                                                   .ToListAsync();
            if (testResultsProcessor is not null)
            {
                testResultsProcessor.setOpenQuesitons(openQuestionAnswers);
                await testResultsProcessor.StartAsync(new CancellationToken());
            }

            try
            {
                var openQuestionsViewModels = await context.OpenQuestionAnswers
                                                       .Where(q => q.StudentId == studentId && q.Question.TestId == testId)
                                                       .Include(q => q.Question)
                                                       .Select(q => new OpenQuestionReviewViewModel()
                                                       {
                                                           Order = q.Question.Order,
                                                           Text = q.Question.Text,
                                                           Id = q.Id,
                                                           RightAnswer = q.Question.Answer,
                                                           MaxScore = q.Question.MaxScore,
                                                           Answer = q.Answer,
                                                           Score = q.Points,
                                                           Explanation = q.Explanation
                                                       })
                                                       .ToListAsync();
                var closedQuestions = new List<ClosedQuestionReviewViewModel>();
                var db = context.ClosedQuestionAnswers
                                .Where(q => q.StudentId == studentId && q.Question.TestId == testId)
                                .Include(q => q.Question);
                foreach (var q in db)
                {
                    var closedQuestionModel = new ClosedQuestionReviewViewModel()
                    {
                        PossibleAnswers = q.Question.Answers.Split("&", System.StringSplitOptions.None),
                        IsDeleted = false,
                        Order = q.Question.Order,
                        Text = q.Question.Text,
                        Id = q.Id,
                        Answers = ProccessAnswerIndexes(q.Question.Answers.Split("&", System.StringSplitOptions.None),
                                                        q.AnswerIndexes),
                        RightAnswers = q.Question.AnswerIndexes.Split("&", System.StringSplitOptions.None).Select(int.Parse)
                                        .ToArray(),
                        MaxScore = q.Question.MaxScore
                    };
                    closedQuestionModel.Score = CalculateClosedQuestionScore(closedQuestionModel.Answers,
                         closedQuestionModel.RightAnswers,
                         closedQuestionModel.MaxScore);
                    closedQuestions.Add(closedQuestionModel);
                }

                return new TestReviewViewModel()
                {
                    OpenQuestions = openQuestionsViewModels,
                    ClosedQuestions = closedQuestions,
                    Score = closedQuestions.Sum(q => q.Score)
                          + openQuestionsViewModels.Sum(q => q.Score)
                };
            }
            catch (InvalidOperationException e)
            {
            }
            return new TestReviewViewModel()
            {
                OpenQuestions = new List<OpenQuestionReviewViewModel>(),
                ClosedQuestions = new List<ClosedQuestionReviewViewModel>()
            };
        }
        public async Task<IEnumerable<TestResultsViewModel>> GetStudentsTestsResults(Guid studentId)
        {
            return await context.TestResults
                                .Where(t => t.StudentId == studentId)
                                .Select(t => ToResultsViewModel(t))
                                .ToListAsync();
        }

    }
}
