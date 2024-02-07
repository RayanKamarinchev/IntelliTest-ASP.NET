using System.Text.RegularExpressions;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Questions.Closed;
using IntelliTest.Core.Models.Questions.Open;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models.Tests.Groups;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

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
                testResultsProcessor =
                    new TestResultsProcessorService(config["OpenAIKey"], config["ConnectionString"], _context);
            }
        }

        private Func<TestResult, TestResultsViewModel> ToResultsViewModel = t => new TestResultsViewModel()
            {
                TakenOn = t.TakenOn,
                Title = t.Group.Test.Title,
                Description = t.Group.Test.Description,
                Grade = t.Group.Test.Grade,
                Mark = t.Mark,
                Score = t.Score,
                TestId = t.Group.TestId
            };

        private List<QuestionType> ProcessQuestionOrder(string questionOrderText)
        {
            return questionOrderText.Split("|", StringSplitOptions.RemoveEmptyEntries)
                                    .Select(q => q == "O" ? QuestionType.Open : QuestionType.Closed)
                                    .ToList();
        }


        public async Task SaveStudentTestAnswer(List<OpenQuestionSubmitViewModel> openQuestions,
            List<ClosedQuestionViewModel> closedQuestions, Guid studentId, Guid groupId)
        {
            if (context.TestResults.Any(t => t.StudentId == studentId && t.GroupId == groupId))
            {
                return;
            }

            var open = openQuestions?.Select(q => new OpenQuestionAnswer()
                                                  {
                                                      Answer = q.Answer,
                                                      QuestionId = q.Id,
                                                      StudentId = studentId,
                                                      Points = q.CorrectAnswer == q.Answer ? q.MaxScore : 0
                                                  });
            var closed = closedQuestions?.Select(q => new ClosedQuestionAnswer()
                                                      {
                                                          AnswerIndexes = string.Join("&", q.AnswerIndexes
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
                                                   GroupId = groupId,
                                                   TakenOn = DateTime.Now
                                               });
            await context.SaveChangesAsync();
        }

        public float CalculateClosedQuestionScore(bool[] Answers, int[] RightAnswers, float MaxScore)
        {
            float score = 0;
            for (int i = 0; i < Answers.Length; i++)
            {
                if (Answers[i] && RightAnswers.Contains(i))
                {
                    score++;
                }
            }

            score *= MaxScore / RightAnswers.Count();
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

        public TestGroupEditViewModel ToEdit(TestViewModel test, RawTestGroupViewModel group)
        {
            var t = new TestGroupEditViewModel()
                    {
                        Groups = test.Groups.Select(g => new TestGroupViewModel()
                                                         {
                                                             Number = g.Number,
                                                             Id = g.Id
                                                         })
                                     .OrderBy(g => g.Number).ToList(),
                        OpenQuestions = group.OpenQuestions
                                             .Where(q => !q.IsDeleted)
                                             .Select(q => new OpenQuestionViewModel()
                                                          {
                                                              Answer = q.Answer,
                                                              IsDeleted = false,
                                                              Text = q.Text,
                                                              MaxScore = q.MaxScore,
                                                              IsEquation = q.IsEquation,
                                                              ImagePath = q.ImagePath
                                                          })
                                             .ToList(),
                        ClosedQuestions = group.ClosedQuestions
                                               .Where(q => !q.IsDeleted)
                                               .Select(q => new ClosedQuestionViewModel()
                                                            {
                                                                Answers = q.Answers.Split("&"),
                                                                AnswerIndexes =
                                                                    ProccessAnswerIndexes(q.Answers.Split("&"),
                                                                        q.AnswerIndexes),
                                                                IsDeleted = false,
                                                                Text = q.Text,
                                                                MaxScore = q.MaxScore,
                                                                IsEquation = q.IsEquation,
                                                                ImagePath = q.ImagePath
                                                            })
                                               .ToList(),
                        QuestionsOrder = ProcessQuestionOrder(group.QuestionsOrder),
                        Time = test.Time,
                        Description = test.Description,
                        Grade = test.Grade,
                        Title = test.Title,
                        Id = test.Id,
                        PublicityLevel = test.PublicityLevel,
                        GroupId = group.Id
                    };
            return t;
        }


        public Guid[] GetExaminersIds(Guid groupId)
        {
            return context.TestResults
                          .Where(tr => tr.GroupId == groupId)
                          .Select(x => x.StudentId)
                          .ToArray();
        }

        public async Task<TestStatsViewModel> GetStatistics(Guid testId)
        {
            var groupIds = context.TestGroups
                                  .Where(t => t.TestId == testId)
                                  .Select(g => g.Id);
            TestStatsViewModel testStatistics = new TestStatsViewModel();

            TestGroupStatsViewModel groupStats = new TestGroupStatsViewModel();
            TestGroup group;
            string testTitle = "";
            bool isFirstIteration = true;
            foreach (var groupId in groupIds)
            {
                var studentIds = GetExaminersIds(groupId);
                List<TestReviewViewModel> res = new List<TestReviewViewModel>();
                foreach (var studentId in studentIds)
                {
                    res.Add(await GetStudentTestResults(testId, studentId));
                }

                groupStats = new TestGroupStatsViewModel();
                if (isFirstIteration)
                {
                    group = await context.TestGroups
                                         .Include(g => g.TestResults)
                                         .Include(g => g.Test)
                                         .FirstOrDefaultAsync(g => g.Id == groupId);
                    testTitle = group.Test.Title;
                    isFirstIteration = false;
                }
                else
                {
                    group = await context.TestGroups
                                         .Include(g => g.TestResults)
                                         .FirstOrDefaultAsync(g => g.Id == groupId);
                }

                groupStats.AverageScore =
                    (float)Math.Round(!group.TestResults.Any() ? 0 : group.TestResults.Average(r => r.Score), 2);
                //groupStats.Title = test.Title;
                groupStats.Examiners = group.TestResults.Count();
                groupStats.QuestionOrder = ProcessQuestionOrder(group.QuestionsOrder);
                groupStats.Number = group.Number;

                List<List<List<int>>> allClosedAnswers = new List<List<List<int>>>();
                res.ForEach(r =>
                {
                    List<List<int>> answers = new List<List<int>>();
                    r.ClosedQuestions.ForEach(q =>
                    {
                        answers.Add(new List<int>());
                        for (int i = 0; i < q.Answers.Length; ++i)
                        {
                            if (q.AnswerIndexes[i])
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
                        groupStats.ClosedQuestions.Add(new ClosedQuestionStatsViewModel()
                                                       {
                                                           StudentAnswers = allClosedAnswers.Select(a => a[i]).ToList(),
                                                           Text = res[0].ClosedQuestions[i].Text,
                                                           Answers = res[0].ClosedQuestions[i].Answers,
                                                           ImagePath = res[0].ClosedQuestions[i].ImagePath
                                                       });
                    }
                }

                List<List<string>> allOpenAnswers = new List<List<string>>();
                res.ForEach(r =>
                {
                    List<string> answers = new List<string>();
                    r.OpenQuestions.ForEach(q => { answers.Add(q.Answer); });
                    allOpenAnswers.Add(answers);
                });
                if (allOpenAnswers.Count > 0)
                {
                    for (int i = 0; i < allOpenAnswers[0].Count; i++)
                    {
                        groupStats.OpenQuestions.Add(new OpenQuestionStatsViewModel()
                                                     {
                                                         StudentAnswers = allOpenAnswers.Select(a => a[i]).ToList(),
                                                         Text = res[0].OpenQuestions[i].Text,
                                                         ImagePath = res[0].OpenQuestions[i].ImagePath
                                                     });
                    }
                }

                //TODO problems
                testStatistics.TestGroups.Add(groupStats);
            }

            testStatistics.Title = testTitle;
            testStatistics.AverageScore = testStatistics.TestGroups.Average(t => t.AverageScore);
            testStatistics.Examiners = testStatistics.TestGroups.Sum(g => g.Examiners);

            return testStatistics;
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

            //TODO statistics
            var res = classDb.ClassTests.Select(async (ct) => await GetStatistics(ct.TestId));
            List<TestStatsViewModel> model = new List<TestStatsViewModel>();
            foreach (var test in res)
            {
                model.Add(await test);
            }

            return model;
        }

        public async Task<TestReviewViewModel> GetStudentTestResults(Guid testId, Guid studentId)
        {
            var testResult = await context.TestResults.Include(r => r.Group)
                                        .FirstOrDefaultAsync(r => r.StudentId == studentId && r.Group.TestId == testId);
            Guid groupId = testResult.GroupId;
            var openQuestionAnswers = await context.OpenQuestionAnswers
                                                   .Include(q => q.Question)
                                                   .Where(q => q.StudentId == studentId
                                                               && q.Question.GroupId == groupId
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
                                                           .Where(q => q.StudentId == studentId &&
                                                                       q.Question.GroupId == groupId)
                                                           .Include(q => q.Question)
                                                           .Select(q => new OpenQuestionReviewViewModel()
                                                                        {
                                                                            Text = q.Question.Text,
                                                                            Id = q.Id,
                                                                            CorrectAnswer = q.Question.Answer,
                                                                            MaxScore = q.Question.MaxScore,
                                                                            Answer = q.Answer,
                                                                            Score = q.Points,
                                                                            Explanation = q.Explanation,
                                                                            ImagePath = q.Question.ImagePath
                                                                        })
                                                           .ToListAsync();
                var closedQuestions = new List<ClosedQuestionReviewViewModel>();
                var db = context.ClosedQuestionAnswers
                                .Where(q => q.StudentId == studentId && q.Question.GroupId == groupId)
                                .Include(q => q.Question);
                foreach (var q in db)
                {
                    var closedQuestionModel = new ClosedQuestionReviewViewModel()
                                              {
                                                  Answers = q.Question.Answers.Split("&",
                                                      System.StringSplitOptions.None),
                                                  IsDeleted = false,
                                                  Text = q.Question.Text,
                                                  Id = q.Id,
                                                  AnswerIndexes = ProccessAnswerIndexes(
                                                      q.Question.Answers.Split("&", System.StringSplitOptions.None),
                                                      q.AnswerIndexes),
                                                  CorrectAnswers = q
                                                                   .Question.AnswerIndexes.Split("&",
                                                                       System.StringSplitOptions.None)
                                                                   .Select(int.Parse)
                                                                   .ToArray(),
                                                  MaxScore = q.Question.MaxScore,
                                                  ImagePath = q.Question.ImagePath
                                              };
                    closedQuestionModel.Score = CalculateClosedQuestionScore(closedQuestionModel.AnswerIndexes,
                        closedQuestionModel.CorrectAnswers,
                        closedQuestionModel.MaxScore);
                    closedQuestions.Add(closedQuestionModel);
                }

                return new TestReviewViewModel()
                       {
                           OpenQuestions = openQuestionsViewModels,
                           ClosedQuestions = closedQuestions,
                           Score = closedQuestions.Sum(q => q.Score)
                                   + openQuestionsViewModels.Sum(q => q.Score),
                           QuestionOrder = testResult.Group.QuestionsOrder
                                                     .Split("|")
                                                     .Select(q => q == "O" ? QuestionType.Open : QuestionType.Closed)
                                                     .ToList()
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
                                .Include(tr => tr.Group)
                                .ThenInclude(g => g.Test)
                                .Where(t => t.StudentId == studentId)
                                .Select(t => ToResultsViewModel(t))
                                .ToListAsync();
        }

        public async Task SubmitTestScore(Guid groupId, Guid studentId, TestReviewViewModel scoredTest)
        {
            var openQuestionAnswers = await context.OpenQuestionAnswers
                                                   .Include(q => q.Question)
                                                   .Where(q => q.StudentId == studentId
                                                               && q.Question.GroupId == groupId)
                                                   .ToListAsync();
            foreach (var openQuestionAnswer in openQuestionAnswers)
            {
                var scoredQuestion = scoredTest.OpenQuestions.FirstOrDefault(q => q.Id == openQuestionAnswer.Id);
                openQuestionAnswer.Explanation = scoredQuestion.Explanation;
                openQuestionAnswer.Points = scoredQuestion.Score;
            }

            await context.SaveChangesAsync();
        }
    }
}