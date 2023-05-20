using System.Text.Json;
using IntelliTest.Data;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Data.Entities;
using Microsoft.EntityFrameworkCore;
using IntelliTest.Core.Models;
using IntelliTest.Data.Enums;
using Org.BouncyCastle.Ocsp;
using System.Diagnostics;
using TiktokenSharp;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace IntelliTest.Core.Services
{
    public class TestService : ITestService
    {
        private readonly IntelliTestDbContext context;
        private readonly IConfiguration config;

        public TestService(IntelliTestDbContext _context, IConfiguration _config)
        {
            context = _context;
            config = _config;
        }

        public async Task<QueryModel<TestViewModel>> Filter(IQueryable<Test> testQuery, QueryModel<TestViewModel> query)
        {
            if (query.Filters.Subject!=Subject.Няма)
            {
                testQuery = testQuery.Where(t => t.Subject == query.Filters.Subject);
            }
            if (query.Filters.Grade >= 1 && query.Filters.Grade <= 12)
            {
                testQuery = testQuery.Where(t => t.Grade == query.Filters.Grade);
            }

            if (string.IsNullOrEmpty(query.Filters.SearchTerm) == false)
            {
                query.Filters.SearchTerm = $"%{query.Filters.SearchTerm.ToLower()}%";

                testQuery = testQuery
                    .Where(t => EF.Functions.Like(t.Title.ToLower(), query.Filters.SearchTerm) ||
                                EF.Functions.Like(t.Creator.School.ToLower(), query.Filters.SearchTerm) ||
                                EF.Functions.Like(t.Description.ToLower(), query.Filters.SearchTerm));
            }
            if (query.Filters.Sorting == Sorting.Likes)
            {
                testQuery = testQuery.OrderBy(t => t.TestLikes.Count());
            }
            else if (query.Filters.Sorting == Sorting.Examiners)
            {
                testQuery = testQuery.OrderBy(t => t.TestResults.Count());
            }
            else if (query.Filters.Sorting == Sorting.Questions)
            {
                testQuery = testQuery.OrderBy(t => t.ClosedQuestions.Count() + t.OpenQuestions.Count());
            }
            else if (query.Filters.Sorting == Sorting.Score)
            {
                testQuery = testQuery.OrderBy(t => t.TestResults.Count() == 0 ? 0 : t.TestResults.Average(r=>r.Score));
            }

            var test = testQuery.Skip(query.ItemsPerPage * (query.CurrentPage - 1))
                                .Take(query.ItemsPerPage)
                                .Select(t => new TestViewModel()
                                {
                                    AverageScore = Math.Round(t.TestResults.Count()==0 ? 0 : t.TestResults.Average(r => r.Score),2),
                                    ClosedQuestions = t.ClosedQuestions,
                                    CreatedOn = t.CreatedOn,
                                    Description = t.Description,
                                    Grade = t.Grade,
                                    Id = t.Id,
                                    MaxScore = t.ClosedQuestions.Sum(q => q.MaxScore) +
                                               t.OpenQuestions.Sum(q => q.MaxScore),
                                    OpenQuestions = t.OpenQuestions,
                                    Time = t.Time,
                                    Title = t.Title,
                                    MultiSubmit = t.MultiSubmission,
                                    PublicityLevel = t.PublicyLevel
                                });
            var tests =await test.ToListAsync();
            query.Items = tests;
            query.TotalItemsCount = tests.Count;
            return query;
        }

        public async Task<QueryModel<TestViewModel>> FilterMine(IEnumerable<Test> testQuery, QueryModel<TestViewModel> query)
        {
            if (query.Filters.Subject != Subject.Няма)
            {
                testQuery = testQuery.Where(t => t.Subject == query.Filters.Subject);
            }
            if (query.Filters.Grade >= 1 && query.Filters.Grade <= 12)
            {
                testQuery = testQuery.Where(t => t.Grade == query.Filters.Grade);
            }

            if (string.IsNullOrEmpty(query.Filters.SearchTerm) == false)
            {
                query.Filters.SearchTerm = query.Filters.SearchTerm.Replace("%", "");
                testQuery = testQuery
                    .Where(t => t.Title.ToLower().Contains(query.Filters.SearchTerm) ||
                                t.Creator.School.ToLower().Contains(query.Filters.SearchTerm) ||
                                t.Description.ToLower().Contains(query.Filters.SearchTerm));
            }
            if (query.Filters.Sorting == Sorting.Likes)
            {
                testQuery = testQuery.OrderBy(t => t.TestLikes.Count());

            }
            else if (query.Filters.Sorting == Sorting.Examiners)
            {
                testQuery = testQuery.OrderBy(t => t.TestResults.Count());
            }
            else if (query.Filters.Sorting == Sorting.Questions)
            {
                testQuery = testQuery.OrderBy(t => t.ClosedQuestions.Count() + t.OpenQuestions.Count());
            }
            else if (query.Filters.Sorting == Sorting.Score)
            {
                testQuery = testQuery.OrderBy(t => t.TestResults.Count() == 0 ? 0 : t.TestResults.Average(r => r.Score));
            }

            var test = testQuery.Skip(query.ItemsPerPage * (query.CurrentPage - 1))
                                .Take(query.ItemsPerPage)
                                .Select(t => new TestViewModel()
                                {
                                    AverageScore = Math.Round(t.TestResults.Count() == 0 ? 0 : t.TestResults.Average(r => r.Score), 2),
                                    ClosedQuestions = t.ClosedQuestions,
                                    CreatedOn = t.CreatedOn,
                                    Description = t.Description,
                                    Grade = t.Grade,
                                    Id = t.Id,
                                    MaxScore = t.ClosedQuestions.Sum(q => q.MaxScore) +
                                               t.OpenQuestions.Sum(q => q.MaxScore),
                                    OpenQuestions = t.OpenQuestions,
                                    Time = t.Time,
                                    Title = t.Title,
                                    MultiSubmit = t.MultiSubmission,
                                    PublicityLevel = t.PublicyLevel
                                });
            query.Items = test.ToList();
            query.TotalItemsCount = test.Count();
            return query;
        }

        public async Task<QueryModel<TestViewModel>> GetAll(bool isTeacher, QueryModel<TestViewModel> query)
        {
            var testQuery = context.Tests
                                   .Include(t=>t.TestResults)
                                   .Include(t => t.TestLikes)
                                   .Where(t => !t.IsDeleted
                                                  && (t.PublicyLevel == PublicityLevel.Public ||
                                                      (isTeacher && t.PublicyLevel == PublicityLevel.TeachersOnly)));
            return await Filter(testQuery, query);
        }

        public async Task<QueryModel<TestViewModel>> GetMy(Guid teacherId, QueryModel<TestViewModel> query)
        {
            var testQuery = context.Tests
                                   .Include(t => t.TestResults)
                                   .Include(t=>t.TestLikes)
                                   .Where(t => !t.IsDeleted
                                                  && t.CreatorId == teacherId);
            return await Filter(testQuery, query);
        }

        public async Task<TestViewModel?> GetById(Guid id)
        {
            var t = await context.Tests
                                 .Where(t=>!t.IsDeleted)
                                 .Include(t=>t.OpenQuestions)
                                 .Include(t=>t.ClosedQuestions)
                                 .Include(t=>t.TestResults)
                                 .Include(t => t.TestLikes)
                                 .FirstOrDefaultAsync(t=>t.Id == id);
            if (t is null)
            {
                return null;
            }
            return new TestViewModel()
            {
                AverageScore = Math.Round(t.TestResults.Count() == 0 ? 0 : t.TestResults.Average(r => r.Score), 2),
                ClosedQuestions = t.ClosedQuestions,
                CreatedOn = t.CreatedOn,
                Description = t.Description,
                Grade = t.Grade,
                Id = t.Id,
                MaxScore = t.ClosedQuestions.Sum(q => q.MaxScore) + t.OpenQuestions.Sum(q => q.MaxScore),
                OpenQuestions = t.OpenQuestions,
                Time = t.Time,
                Title = t.Title,
                MultiSubmit = t.MultiSubmission,
                PublicityLevel = t.PublicyLevel
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
                Id = model.Id,
                PublicityLevel = model.PublicityLevel
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


        public async Task Edit(Guid id, TestEditViewModel model, Guid teacherId)
        {
            var test = await context.Tests
                                    .Include(t=>t.OpenQuestions)
                                    .Include(t=>t.ClosedQuestions)
                                    .FirstOrDefaultAsync(t=>t.Id==id);
            
            test.OpenQuestions = test.OpenQuestions.Select(x =>
            {
                var modelQuestion = model.OpenQuestions.FirstOrDefault(q => q.Answer == x.Answer || q.Text == x.Text);
                if (modelQuestion is null)
                {
                    return new OpenQuestion();
                }
                model.OpenQuestions.Remove(modelQuestion);
                x.Text = modelQuestion.Text;
                x.Answer = modelQuestion.Answer;
                x.Order = modelQuestion.Order;
                x.MaxScore = modelQuestion.MaxScore;
                return x;
            })
                                     .Where(q=>q.Text!="")
                                     .Union(model.OpenQuestions
                                                 .Select(q => new OpenQuestion()
                                                 {
                                                     Text = q.Text,
                                                     Answer = q.Answer,
                                                     Order = q.Order,
                                                     MaxScore = q.MaxScore
                                                 }))
                                     .ToList();

            test.ClosedQuestions = test.ClosedQuestions.Select(x =>
                                     {
                                         var modelQuestion = model.ClosedQuestions
                                                                  .FirstOrDefault(q =>
                                                                                      string.Join("&", q.Answers.Where(a => !string.IsNullOrEmpty(a)))
                                                                                   == x.Answers || q.Text == x.Text);
                                         if (modelQuestion is null)
                                         {
                                             return new ClosedQuestion();
                                         }
                                         model.ClosedQuestions.Remove(modelQuestion);
                                         x.Text = modelQuestion.Text;
                                         x.Answers = string.Join(
                                             "&", modelQuestion.Answers.Where(a => !string.IsNullOrEmpty(a)));
                                         x.Order = modelQuestion.Order;
                                         x.AnswerIndexes = string.Join("&", modelQuestion.AnswerIndexes
                                                                                         .Select((val, indx) =>
                                                                                                     new { val, indx })
                                                                                         .Where(q => q.val)
                                                                                         .Select(q => q.indx));
                                         x.MaxScore = modelQuestion.MaxScore;
                                         return x;
                                     })
                                     .Where(q => q.Text != "")
                                     .Union(model.ClosedQuestions
                                                 .Select(q => new ClosedQuestion()
                                                 {
                                                     Text = q.Text,
                                                     AnswerIndexes = string.Join("&", q.AnswerIndexes
                                                                                       .Select((val, indx) => new { val, indx })
                                                                                       .Where(q => q.val)
                                                                                       .Select(q => q.indx)),
                                                     Answers = string.Join("&", q.Answers.Where(a => !string.IsNullOrEmpty(a))),
                                                     Order = q.Order,
                                                     MaxScore = q.MaxScore
                                                 }))
                                     .ToList();

            if (test.CreatorId==teacherId)
            {
                test.Title = model.Title;
                test.Description = model.Description;
                test.Grade = model.Grade;
                test.Time = model.Time;
                test.PublicyLevel = model.PublicityLevel;
                //context.Update(test);
            }
            else
            {
                var newTest = new Test()
                {
                    Title = model.Title,
                    Description = model.Description,
                    Grade = model.Grade,
                    Time = model.Time,
                    ClosedQuestions = test.ClosedQuestions,
                    OpenQuestions = test.OpenQuestions,
                    CreatedOn = DateTime.Now,
                    CreatorId = teacherId,
                    PublicyLevel = model.PublicityLevel
                };
                context.Tests.Add(newTest);
            }
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

        public string Translate(string text)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"C:\Users\raian\AppData\Local\Programs\Python\Python38\python.exe";
            start.Arguments = string.Format("translate.py \"{0}\"", text);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            string last = "";
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    last = reader.ReadToEnd();
                }
            }
            return last;
        }

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        public async Task<Tuple<decimal, string>> CalculateOpenQuestionScore(string Answer, string RightAnswer, int MaxScore)
        {
            if (Regex.Matches(RightAnswer, @"\w+")
                     .Select(m => m.Value).Count() <= 4)
            {
                bool isCorrect = Regex.Matches(Answer, @"\w+")
                                      .Select(m => m.Value)
                                      .SequenceEqual(Regex.Matches(RightAnswer, @"\w+")
                                                          .Select(m => m.Value));
                return new Tuple<decimal, string>(isCorrect ? MaxScore : 0, $"Правилният отговор беше \"{RightAnswer}\"");
            }
            var api = new OpenAI_API.OpenAIAPI(config["OpenAIKey"]);
            var chat = api.Chat.CreateConversation();

            string translatedCorrectAnswer = Translate(RightAnswer);
            string translatedAnswer = Translate(Answer);

            TikToken tikToken = TikToken.EncodingForModel("gpt-3.5-turbo");
            var encodedCorrect = tikToken.Encode(translatedCorrectAnswer);
            var partOfCorrect = encodedCorrect.Take(2000).ToList();
            var encodedAnswer = tikToken.Encode(translatedAnswer);
            var partOfAnswer = encodedAnswer.Take(2000).ToList();
            string text = "Correct answer \"" + tikToken.Decode(partOfCorrect) + "\" Given answer \"" + tikToken.Decode(partOfAnswer) + $"Answer with \"Score: (score out of {MaxScore*2}) Message: (message in bulgarian)\"";
            chat.AppendUserInput(text);
            var response = await chat.GetResponseFromChatbotAsync();
            decimal score = decimal.Parse(Regex.Match(response, @"\d+").Value)/2;
            string message = response.Substring(response.IndexOf(':', response.IndexOf(':') + 3))
                                     .Replace('\n', ' ');
            return new Tuple<decimal, string>(score, message);
        }

        public async Task<TestReviewViewModel> TestResults(Guid testId, Guid studentId)
        {
            if (!context.OpenQuestionAnswers
                      .Where(q => q.StudentId == studentId && q.Question.TestId == testId)
                      .Any(q=>q.Explanation!="")
               )
            {
                var openQuestions = await context.OpenQuestionAnswers
                                                 .Where(q => q.StudentId == studentId && q.Question.TestId == testId)
                                                 .Include(q => q.Question)
                                                 .ToListAsync();
                foreach (var q in openQuestions)
                {
                    var eval = 
                        await CalculateOpenQuestionScore(q.Answer, q.Question.Answer, q.Question.MaxScore);
                    q.Points = eval.Item1;
                    q.Explanation = eval.Item2;
                }

                await context.SaveChangesAsync();
            }
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

        public async Task AddTestAnswer(List<OpenQuestionAnswerViewModel> openQuestions,
                                        List<ClosedQuestionAnswerViewModel> closedQuestions, Guid studentId, Guid testId)
        {
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
            
            var review = await TestResults(testId, studentId);

            await context.TestResults.AddAsync(new TestResult()
            {
                Mark = Mark.Unmarked,
                Score = review.Score,
                StudentId = studentId,
                TestId = testId,
                TakenOn = DateTime.Now
            });

        }
        //
        public bool[] ProccessAnswerIndexes(string[] answers, string answerIndexes)
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

        public async Task<bool> IsTestTakenByStudentId(Guid testId, Student student)
        {
            bool closed = (student?.ClosedAnswers?.Any(a => a?.Question?.Test?.Id == testId) ?? false);
            bool open = (student?.OpenAnswers?.Any(a => a?.Question?.Test?.Id == testId) ?? false);
            return closed || open;
        }

        public async Task<TestStatsViewModel> GetStatistics(Guid testId)
        {
            var studentIds = GetExaminersIds(testId);
            List<TestReviewViewModel> res = new List<TestReviewViewModel>();
            foreach (var studentId in studentIds)
            {
                res.Add(await TestResults(testId, studentId));
            }

            TestStatsViewModel model = new TestStatsViewModel();
            var test = await GetById(testId);
            model.AverageScore = test.AverageScore;
            model.Title = test.Title;
            model.Examiners = test.Students;

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
            if (allClosedAnswers.Count>0)
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

        public async Task<QueryModel<TestViewModel>> TestsTakenByStudent(Guid studentId, QueryModel<TestViewModel> query)
        {
            var student = await context.Students
                                       .Include(s=>s.ClosedAnswers)
                                       .ThenInclude(a=>a.Question)
                                       .ThenInclude(q=>q.Test)
                                       .ThenInclude(t=>t.TestResults)
                                       

                                       .Include(s => s.ClosedAnswers)
                                       .ThenInclude(a => a.Question)
                                       .ThenInclude(q => q.Test)
                                       .ThenInclude(t=>t.TestLikes)

                                       .Include(s=>s.OpenAnswers)
                                       .ThenInclude(a => a.Question)
                                       .ThenInclude(q => q.Test)
                                       .ThenInclude(t=>t.TestResults)

                                       .Include(s => s.OpenAnswers)
                                       .ThenInclude(a => a.Question)
                                       .ThenInclude(q => q.Test)
                                       .ThenInclude(t=>t.TestLikes)

                                       .FirstOrDefaultAsync(s=>s.Id == studentId);
            var closedQuestionTests = student?.ClosedAnswers
                                 ?.Select(a => a.Question.Test)
                                 ?.Distinct() ?? new List<Test>();
            var openQuestionTests = student?.OpenAnswers
                                  ?.Select(a => a.Question.Test)
                                  ?.Distinct() ?? new List<Test>();
            var testsQuery = closedQuestionTests.Union(openQuestionTests);
            return await FilterMine(testsQuery, query);
        }

        public async Task<Guid> Create(TestViewModel model, Guid teacherId, string[] classNames)
        {
            Test test = new Test()
            {
                Title = model.Title,
                Description = model.Description,
                Subject = model.Subject,
                Time = model.Time,
                Grade = model.Grade,
                CreatedOn = DateTime.Now,
                CreatorId = teacherId,
                OpenQuestions = new List<OpenQuestion>(),
                ClosedQuestions = new List<ClosedQuestion>(),
                PhotoPath = model.PhotoPath
            };
            var classes = await context.Classes.Where(c => classNames.Contains(c.Name)).ToListAsync();
            var e = await context.Tests.AddAsync(test);
            await context.ClassTests.AddRangeAsync(classes.Select(c => new ClassTest()
            {
                Class = c,
                Test = test
            }));
            await context.SaveChangesAsync();
            return e.Entity.Id;
        }

        public async Task<bool> ExistsbyId(Guid id)
        {
            return await context.Tests
                                .Where(c => !c.IsDeleted)
                                .AnyAsync(t=>t.Id == id);
        }

        public async Task<bool> StudentHasAccess(Guid testId, Guid studentId)
        {
            Test? test = await context.Tests
                                      .Where(t=>!t.IsDeleted)
                                      .Include(t=>t.ClassesWithAccess)
                                      .ThenInclude(ct=>ct.Class)
                                      .ThenInclude(c=>c.Students)
                                      .FirstOrDefaultAsync(t => t.Id == testId);
            if (test == null)
            {
                return false;
            }

            if (test.PublicyLevel == PublicityLevel.Public)
            {
                return true;
            }

            if (test.PublicyLevel == PublicityLevel.TeachersOnly)
            {
                return false;
            }

            if (test.PublicyLevel==PublicityLevel.ClassOnly)
            {
                return test.ClassesWithAccess.Any(c => c.Class.Students.Any(s => s.StudentId == studentId));
            }

            return false;
        }

        public async Task DeleteTest(Guid id)
        {
            var test = await context.Tests.FindAsync(id);
            test.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task<List<TestStatsViewModel>> TestsTakenByClass(Guid classId)
        {
            Class? classDb = await context.Classes
                                       .Include(c=>c.ClassTests)
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
    }
}
