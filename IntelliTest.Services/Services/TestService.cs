using IntelliTest.Data;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Data.Entities;
using Microsoft.EntityFrameworkCore;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Users;
using IntelliTest.Data.Enums;
using Microsoft.Extensions.Configuration;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions.Closed;

namespace IntelliTest.Core.Services
{
    public class TestService : ITestService
    {
        private readonly IntelliTestDbContext context;

        public TestService(IntelliTestDbContext _context)
        {
            context = _context;
        }

        private Func<Test, TestViewModel> ToViewModel = t => new TestViewModel()
        {
            AverageScore = Math.Round(!t.TestResults.Any() ? 0 : t.TestResults.Average(r => r.Score), 2),
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
            PublicityLevel = t.PublicyLevel,
            Students = t.TestResults.Count(),
            QuestionOrder = t.QuestionsOrder
        };

        private List<QuestionType> ProcessQuestionOrder(string questionOrderText)
        {
            return questionOrderText.Split("|")
                                    .Select(q => q == "O" ? QuestionType.Open : QuestionType.Closed)
                                    .ToList();
        }


        public async Task<QueryModel<TestViewModel>> Filter(IQueryable<Test> testQuery, QueryModel<TestViewModel> query, Guid? teacherId, Guid? studentId)
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
                                .Include(t=>t.ClosedQuestions)
                                .Include(t=>t.OpenQuestions)
                                .Include(t=>t.TestResults)
                                .Select(x=>ToViewModel(x));
            var tests =await test.ToListAsync();
            foreach (var t in tests)
            {
                 t.IsOwner = false;
                 if (teacherId is not null)
                     t.IsOwner = await IsTestCreator(t.Id, teacherId.Value);

                 t.IsTestTaken = false;
                 if (studentId is not null)
                     t.IsTestTaken = await IsTestTakenByStudentId(t.Id, studentId.Value);
             }
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
                                .Select(x=>ToViewModel(x))
                                .ToList();
            test.ForEach(t => t.IsTestTaken = true);
            query.Items = test.ToList();
            query.TotalItemsCount = test.Count();
            return query;
        }

        public async Task<QueryModel<TestViewModel>> GetAll(Guid? teacherId, Guid? studentId, QueryModel<TestViewModel> query)
        {
            var testQuery = context.Tests
                                   .Include(t=>t.TestResults)
                                   .Include(t => t.TestLikes)
                                   .Where(t => !t.IsDeleted
                                                  && (t.PublicyLevel == PublicityLevel.Public ||
                                                      (teacherId.ToString()!="" && t.PublicyLevel == PublicityLevel.TeachersOnly)));
            return await Filter(testQuery, query, teacherId, studentId);
        }

        public async Task<QueryModel<TestViewModel>> GetMy(Guid? teacherId, Guid? studentId, QueryModel<TestViewModel> query)
        {
            var testQuery = context.Tests
                                   .Include(t => t.TestResults)
                                   .Include(t=>t.TestLikes)
                                   .Where(t => !t.IsDeleted
                                                  && t.CreatorId == teacherId);
            return await Filter(testQuery, query, teacherId, studentId);
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

            return ToViewModel(t);
        }

        public TestSubmitViewModel ToSubmit(TestViewModel model)
        {
            var t = new TestSubmitViewModel()
            {
                OpenQuestions = model.OpenQuestions
                                     .Where(q => !q.IsDeleted)
                                     .Select(q => new OpenQuestionAnswerViewModel()
                                     {
                                         Text = q.Text,
                                         Id = q.Id,
                                         MaxScore = q.MaxScore,
                                         ImagePath = q.ImagePath,
                                         IsEquation = q.IsEquation
                                     })
                                     .ToList(),
                ClosedQuestions = model.ClosedQuestions
                                       .Where(q => !q.IsDeleted)
                                       .Select(q => new ClosedQuestionAnswerViewModel()
                                       {
                                           PossibleAnswers = q.Answers.Split("&"),
                                           IsDeleted = false,
                                           Text = q.Text,
                                           Id = q.Id,
                                           MaxScore = q.MaxScore,
                                           ImagePath = q.ImagePath,
                                           IsEquation = q.IsEquation,
                                           Answers = new bool[q.Answers.Length]
                                       })
                                       .ToList(),
                Time = model.Time,
                Title = model.Title,
                QuestionOrder = ProcessQuestionOrder(model.QuestionOrder),
                Id = model.Id
            };
            return t;
        }

        public async Task Edit(Guid id, TestEditViewModel model, Guid teacherId)
        {
            var test = await context.Tests
                                    .Include(t=>t.OpenQuestions)
                                    .Include(t=>t.ClosedQuestions)
                                    .FirstOrDefaultAsync(t=>t.Id==id);

            test.OpenQuestions = test.OpenQuestions.Select(q => EditOpenQuestion(model.OpenQuestions, q))
                                     .Where(q => !string.IsNullOrEmpty(q.Text))
                                     .Union(model.OpenQuestions
                                                 .Select(q => new OpenQuestion()
                                                 {
                                                     Text = q.Text,
                                                     Answer = q.Answer,
                                                     MaxScore = q.MaxScore,
                                                     ImagePath = q.ImagePath,
                                                     IsEquation = q.IsEquation
                                                 }))
                                     .ToList();
            
            test.ClosedQuestions = test.ClosedQuestions.Select(q => EditClosedQuestion(model.ClosedQuestions, q))
                                     .Where(q => !string.IsNullOrEmpty(q.Text))
                                     .Union(model.ClosedQuestions
                                                 .Select(q => new ClosedQuestion()
                                                 {
                                                     Text = q.Text,
                                                     AnswerIndexes = string.Join("&", q.AnswerIndexes
                                                                                       .Select((val, indx) =>
                                                                                                   new { val, indx })
                                                                                       .Where(q => q.val)
                                                                                       .Select(q => q.indx)),
                                                     Answers = string.Join(
                                                         "&", q.Answers.Where(a => !string.IsNullOrEmpty(a))),
                                                     MaxScore = q.MaxScore,
                                                     ImagePath = q.ImagePath,
                                                     IsEquation = q.IsEquation
                                                 }))
                                     .ToList();

            if (test.CreatorId==teacherId)
            {
                test.Title = model.Title;
                test.Description = model.Description;
                test.Grade = model.Grade;
                test.Time = model.Time;
                test.PublicyLevel = model.PublicityLevel;
                test.QuestionsOrder = string.Join('|', model.QuestionsOrder.Select(q => q.ToString()[0]));
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
                    PublicyLevel = model.PublicityLevel,
                    QuestionsOrder = string.Join('|', model.QuestionsOrder.Select(q => q.ToString()[0]))
                };
                context.Tests.Add(newTest);
            }
            await context.SaveChangesAsync();
        }

        private Func<List<OpenQuestionViewModel>, OpenQuestion, OpenQuestion> EditOpenQuestion = (allQuestions, question) =>
        {
            var testQuestion = allQuestions.FirstOrDefault(q => q.Answer == question.Answer || q.Text == question.Text);
            if (testQuestion is null)
            {
                return new OpenQuestion();
            }
            allQuestions.Remove(testQuestion);
            question.Text = testQuestion.Text;
            question.Answer = testQuestion.Answer;
            question.MaxScore = testQuestion.MaxScore;
            question.ImagePath = testQuestion.ImagePath;
            question.IsEquation = testQuestion.IsEquation;
            return question;
        };
        private Func<List<ClosedQuestionViewModel>, ClosedQuestion, ClosedQuestion> EditClosedQuestion = (allQuestions, question) =>
        {
            var modelQuestion = allQuestions
                                     .FirstOrDefault(q => CheckForSameAnswers(q, question.Answers) || q.Text == question.Text);
            if (modelQuestion is null)
            {
                return new ClosedQuestion();
            }
            allQuestions.Remove(modelQuestion);

            question.Text = modelQuestion.Text;
            question.Answers = string.Join(
                "&", modelQuestion.Answers.Where(a => !string.IsNullOrEmpty(a)));

            question.AnswerIndexes = string.Join("&", modelQuestion.AnswerIndexes
                                                                   .Select((val, indx) =>
                                                                               new { val, indx })
                                                                   .Where(q => q.val)
                                                                   .Select(q => q.indx));
            question.MaxScore = modelQuestion.MaxScore;
            question.ImagePath = modelQuestion.ImagePath;
            question.IsEquation = modelQuestion.IsEquation;
            return question;
        };

        private static bool CheckForSameAnswers(ClosedQuestionViewModel questionViewModel, string savedQuestionAnswers)
        {
            return string.Join("&", questionViewModel.Answers.Where(a => !string.IsNullOrEmpty(a))) == savedQuestionAnswers;
        }

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsTestTakenByStudentId(Guid testId, Guid studentId)
        {
            return context.TestResults.Any(t => t.StudentId == studentId && t.TestId == testId);
        }


        public async Task<QueryModel<TestViewModel>> TestsTakenByStudent(Guid studentId, QueryModel<TestViewModel> query)
        {
            var testsQuery = context.TestResults
                                    .Include(t=>t.Test)
                                    .ThenInclude(t=>t.TestResults)

                                    .Include(t => t.Test)
                                    .ThenInclude(t => t.TestLikes)

                                    .Include(t => t.Test)
                                    .ThenInclude(t => t.TestResults)

                                    .Where(r => r.StudentId == studentId)
                                    .Select(tr => tr.Test);
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
                PhotoPath = "",
                QuestionsOrder = ""
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

        public async Task<bool> IsTestCreator(Guid testId, Guid teacherId)
        {
            var teacher = await context.Teachers
                                       .Include(t => t.Tests)
                                       .FirstOrDefaultAsync(t => t.Id == teacherId);
            if (teacher is null)
            {
                return false;
            }
            return teacher.Tests.Any(t => t.Id == testId);

        }

        public async Task<QueryModel<TestViewModel>> GetAllAdmin(QueryModel<TestViewModel> query)
        {
            var testQuery = context.Tests
                                   .Include(t => t.TestResults)
                                   .Include(t => t.TestLikes)
                                   .Where(t => !t.IsDeleted);
            return await Filter(testQuery, query, null, null);
        }
    }
}
