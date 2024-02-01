using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
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
using TestGroupSubmitViewModel = IntelliTest.Core.Models.Tests.Groups.TestGroupSubmitViewModel;

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
                                                                 AverageScore =
                                                                     (float)Math.Round(
                                                                         !t.Groups.Any(g => g.TestResults.Any())
                                                                             ? 0
                                                                             : t.Groups.Average(g =>
                                                                                 g.TestResults.Average(r => r.Score)),
                                                                         2),
                                                                 Groups = t.Groups.Select(x => new TestGroup()
                                                                     {
                                                                         Id = x.Id,
                                                                         ClosedQuestions = x.ClosedQuestions,
                                                                         OpenQuestions = x.OpenQuestions,
                                                                         QuestionsOrder = x.QuestionsOrder
                                                                     }).ToArray(),
                                                                 CreatedOn = t.CreatedOn,
                                                                 Description = t.Description,
                                                                 Grade = t.Grade,
                                                                 Id = t.Id,
                                                                 Time = t.Time,
                                                                 Title = t.Title,
                                                                 MultiSubmit = t.MultiSubmission,
                                                                 PublicityLevel = t.PublicyLevel,
                                                                 Examiners = t.Groups.Sum(g => g.TestResults.Count())
                                                             };

        private Func<TestGroup, RawTestGroupViewModel> ToRawViewModel = t => new RawTestGroupViewModel()
                                                                             {
                                                                                 OpenQuestions =
                                                                                     t.OpenQuestions.ToList(),
                                                                                 ClosedQuestions =
                                                                                     t.ClosedQuestions.ToList(),
                                                                                 Id = t.Id,
                                                                                 Number = t.Number,
                                                                                 QuestionsOrder = t.QuestionsOrder,
                                                                                 TestId = t.TestId,
                                                                                 TestTitle = t.Test.Title,
                                                                                 Time = t.Test.Time
                                                                             };

        private List<QuestionType> ProcessQuestionOrder(string questionOrderText)
        {
            return questionOrderText.Split("|")
                                    .Select(q => q == "O" ? QuestionType.Open : QuestionType.Closed)
                                    .ToList();
        }


        public async Task<QueryModel<TestViewModel>> Filter(IQueryable<Test> testQuery, QueryModel<TestViewModel> query,
            Guid? teacherId, Guid? studentId)
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
                query.Filters.SearchTerm = $"%{query.Filters.SearchTerm.ToLower()}%";

                testQuery = testQuery
                    .Where(t => EF.Functions.Like(t.Title.ToLower(), query.Filters.SearchTerm) ||
                                EF.Functions.Like(t.Creator.School.ToLower(), query.Filters.SearchTerm) ||
                                EF.Functions.Like(t.Description.ToLower(), query.Filters.SearchTerm));
            }

            //if (query.Filters.Sorting == Sorting.Likes)
            //{
            //    testQuery = testQuery.OrderBy(t => t.TestLikes.Count());
            //}
            if (query.Filters.Sorting == Sorting.Examiners)
            {
                testQuery = testQuery.OrderByDescending(t => t.Groups.Sum(g => g.TestResults.Count()));
            }
            else if (query.Filters.Sorting == Sorting.Questions)
            {
                testQuery = testQuery.OrderByDescending(t =>
                    t.Groups.FirstOrDefault() == null
                        ? 0
                        : (t.Groups.FirstOrDefault().ClosedQuestions.Count +
                           t.Groups.FirstOrDefault().OpenQuestions.Count));
            }
            else if (query.Filters.Sorting == Sorting.Score)
            {
                testQuery = testQuery.OrderByDescending(t =>
                    t.Groups.Any(g => g.TestResults.Any())
                        ? t.Groups.Average(g => g.TestResults.Average(r => r.Score))
                        : 0);
            }

            var test = testQuery.Skip(query.ItemsPerPage * (query.CurrentPage - 1))
                                .Take(query.ItemsPerPage)
                                .Include(t => t.Groups)
                                .ThenInclude(t => t.ClosedQuestions)
                                .Include(t => t.Groups)
                                .ThenInclude(t => t.OpenQuestions)
                                .Include(t => t.Groups)
                                .ThenInclude(g => g.TestResults)
                                .Select(x => ToViewModel(x));
            var tests = await test.ToListAsync();
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

        public async Task<QueryModel<TestViewModel>> FilterMine(IEnumerable<Test> testQuery,
            QueryModel<TestViewModel> query)
        {
            //TODO merger filter qeuries
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
            //if (query.Filters.Sorting == Sorting.Likes)
            //{
            //    testQuery = testQuery.OrderBy(t => t.TestLikes.Count());

            //}
            if (query.Filters.Sorting == Sorting.Examiners)
            {
                testQuery = testQuery.OrderByDescending(t => t.Groups.Sum(g => g.TestResults.Count()));
            }
            else if (query.Filters.Sorting == Sorting.Questions)
            {
                testQuery = testQuery.OrderByDescending(t =>
                    t.Groups.FirstOrDefault() == null
                        ? 0
                        : (t.Groups.FirstOrDefault().ClosedQuestions.Count +
                           t.Groups.FirstOrDefault().OpenQuestions.Count));
            }
            else if (query.Filters.Sorting == Sorting.Score)
            {
                testQuery = testQuery.OrderByDescending(t =>
                    t.Groups.Any(g => g.TestResults.Any())
                        ? t.Groups.Average(g => g.TestResults.Average(r => r.Score))
                        : 0);
            }

            var test = testQuery.Skip(query.ItemsPerPage * (query.CurrentPage - 1))
                                .Take(query.ItemsPerPage)
                                .Select(x => ToViewModel(x))
                                .ToList();
            test.ForEach(t => t.IsTestTaken = true);
            query.Items = test.ToList();
            query.TotalItemsCount = test.Count();
            return query;
        }

        public async Task<QueryModel<TestViewModel>> GetAll(Guid? teacherId, Guid? studentId,
            QueryModel<TestViewModel> query)
        {
            var testQuery = context.Tests
                                   .Include(t=>t.Groups)
                                   .ThenInclude(g => g.TestResults)
                                   .Include(t => t.TestLikes)
                                   .Where(t => !t.IsDeleted
                                               && (t.PublicyLevel == PublicityLevel.Public ||
                                                   (teacherId.ToString() != "" &&
                                                    t.PublicyLevel == PublicityLevel.TeachersOnly)));
            return await Filter(testQuery, query, teacherId, studentId);
        }

        public async Task<QueryModel<TestViewModel>> GetMy(Guid? teacherId, Guid? studentId,
            QueryModel<TestViewModel> query)
        {
            var testQuery = context.Tests
                                   .Include(t => t.Groups)
                                   .ThenInclude(g => g.TestResults)
                                   .Include(t => t.TestLikes)
                                   .Where(t => !t.IsDeleted
                                               && t.CreatorId == teacherId);
            return await Filter(testQuery, query, teacherId, studentId);
        }

        public async Task<TestViewModel?> GetById(Guid id)
        {
            var t = await context.Tests
                                 .Where(t => !t.IsDeleted)
                                 .Include(t => t.Groups)
                                 .ThenInclude(t => t.OpenQuestions)
                                 .Include(t => t.Groups)
                                 .ThenInclude(t => t.ClosedQuestions)
                                 .Include(t => t.Groups)
                                 .ThenInclude(g => g.TestResults)
                                 .Include(t => t.TestLikes)
                                 .FirstOrDefaultAsync(t => t.Id == id);
            if (t is null)
            {
                return null;
            }

            return ToViewModel(t);
        }

        public async Task<RawTestGroupViewModel> GetGroupById(Guid id)
        {
            var g = await context.TestGroups
                                 .Where(t => !t.IsDeleted)
                                 .Include(t => t.OpenQuestions)
                                 .Include(t => t.ClosedQuestions)
                                 .Include(t => t.Test)
                                 .FirstOrDefaultAsync(t => t.Id == id);
            if (g is null)
            {
                return null;
            }

            return ToRawViewModel(g);
        }

        public TestGroupSubmitViewModel ToSubmit(RawTestGroupViewModel model)
        {
            var t = new TestGroupSubmitViewModel()
                    {
                        OpenQuestions = model.OpenQuestions
                                             .Where(q => !q.IsDeleted)
                                             .Select(q => new OpenQuestionSubmitViewModel()
                                                          {
                                                              Text = q.Text,
                                                              Id = q.Id,
                                                              MaxScore = q.MaxScore,
                                                              ImagePath = q.ImagePath,
                                                              IsEquation = q.IsEquation,
                                                              CorrectAnswer = q.Answer
                                                          })
                                             .ToList(),
                        ClosedQuestions = model.ClosedQuestions
                                               .Where(q => !q.IsDeleted)
                                               .Select(q => new ClosedQuestionViewModel()
                                                            {
                                                                Answers = q.Answers.Split("&"),
                                                                IsDeleted = false,
                                                                Text = q.Text,
                                                                Id = q.Id,
                                                                MaxScore = q.MaxScore,
                                                                ImagePath = q.ImagePath,
                                                                IsEquation = q.IsEquation,
                                                                AnswerIndexes =
                                                                    new bool[q.Answers.Count(x => x == '&') + 1]
                                                            })
                                               .ToList(),
                        QuestionOrder = ProcessQuestionOrder(model.QuestionsOrder),
                        Time = model.Time,
                        Title = model.TestTitle,
                        Id = model.Id
                    };
            return t;
        }

        public async Task Edit(Guid id, TestGroupEditViewModel model, Guid? teacherId, bool isAdmin = false)
        {
            var test = await context.Tests
                                    .Include(t => t.Groups)
                                    .ThenInclude(t => t.OpenQuestions)
                                    .Include(t => t.Groups)
                                    .ThenInclude(t => t.ClosedQuestions)
                                    .FirstOrDefaultAsync(t => t.Id == id);

            var dbGroup = test.Groups.First(g => g.Number == model.Number);
            dbGroup.OpenQuestions = dbGroup.OpenQuestions.Select(q => EditOpenQuestion(model.OpenQuestions, q))
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

            dbGroup.ClosedQuestions = dbGroup.ClosedQuestions.Select(q => EditClosedQuestion(model.ClosedQuestions, q))
                                             .Where(q => !string.IsNullOrEmpty(q.Text))
                                             .Union(model.ClosedQuestions
                                                         .Select(q => new ClosedQuestion()
                                                                      {
                                                                          Text = q.Text,
                                                                          AnswerIndexes = string.Join("&",
                                                                              q.AnswerIndexes
                                                                               .Select((val, indx) =>
                                                                                   new { val, indx })
                                                                               .Where(q => q.val)
                                                                               .Select(q => q.indx)),
                                                                          Answers = string.Join(
                                                                              "&",
                                                                              q.Answers.Where(a =>
                                                                                  !string.IsNullOrEmpty(a))),
                                                                          MaxScore = q.MaxScore,
                                                                          ImagePath = q.ImagePath,
                                                                          IsEquation = q.IsEquation
                                                                      }))
                                             .ToList();
            dbGroup.QuestionsOrder = string.Join('|', model.QuestionsOrder.Select(q => q.ToString()[0]));

            test.Title = model.Title;
            test.Description = model.Description;
            test.Grade = model.Grade;
            test.Time = model.Time;
            test.PublicyLevel = model.PublicityLevel;
            //TODO: code for making a copy of a test
            //if (isAdmin || test.CreatorId==teacherId)
            //{
            //}
            //else
            //{
            //    var newTest = new Test()
            //    {
            //        Title = model.Title,
            //        Description = model.Description,
            //        Grade = model.Grade,
            //        Time = model.Time,
            //        ClosedQuestions = test.ClosedQuestions,
            //        OpenQuestions = test.OpenQuestions,
            //        CreatedOn = DateTime.Now,
            //        CreatorId = (Guid)teacherId,
            //        PublicyLevel = model.PublicityLevel,
            //        QuestionsOrder = string.Join('|', model.QuestionsOrder.Select(q => q.ToString()[0]))
            //    };
            //    context.Tests.Add(newTest);
            //}
            await context.SaveChangesAsync();
        }

        private Func<List<OpenQuestionViewModel>, OpenQuestion, OpenQuestion> EditOpenQuestion =
            (allQuestions, question) =>
            {
                var testQuestion =
                    allQuestions.FirstOrDefault(q => q.Answer == question.Answer || q.Text == question.Text);
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

        private Func<List<ClosedQuestionViewModel>, ClosedQuestion, ClosedQuestion> EditClosedQuestion =
            (allQuestions, question) =>
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
            return string.Join("&", questionViewModel.Answers.Where(a => !string.IsNullOrEmpty(a))) ==
                   savedQuestionAnswers;
        }

        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsTestTakenByStudentId(Guid testId, Guid studentId)
        {
            return context.TestResults
                          .Include(t => t.Group)
                          .Any(t => t.StudentId == studentId && t.Group.TestId == testId);
        }


        public async Task<QueryModel<TestViewModel>> TestsTakenByStudent(Guid studentId,
            QueryModel<TestViewModel> query)
        {
            //TODO TestREsults
            var testsQuery = context.TestResults
                                    .Include(t => t.Group)
                                    .ThenInclude(g => g.Test)
                                    .Include(t => t.Group)
                                    .ThenInclude(g => g.Test)
                                    .ThenInclude(t => t.TestLikes)
                                    .Include(t => t.Group)
                                    .ThenInclude(g => g.Test)
                                    .Include(t => t.Group)
                                    .ThenInclude(t => t.TestResults)
                                    .Where(r => r.StudentId == studentId)
                                    .Select(tr => tr.Group.Test);
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
                            Groups = new List<TestGroup>()
                                     {
                                         new TestGroup()
                                         {
                                             OpenQuestions = new List<OpenQuestion>(),
                                             ClosedQuestions = new List<ClosedQuestion>(),
                                             QuestionsOrder = ""
                                         }
                                     },
                            PhotoPath = ""
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

        public async Task<bool> TestExistsbyId(Guid id)
        {
            return await context.Tests
                                .Where(c => !c.IsDeleted)
                                .AnyAsync(t => t.Id == id);
        }

        public async Task<bool> GroupExistsbyId(Guid? id)
        {
            if (id == null)
            {
                return true;
            }

            return await context.TestGroups
                                .Where(c => !c.IsDeleted)
                                .AnyAsync(t => t.Id == id);
        }

        public async Task<bool> StudentHasAccess(Guid testId, Guid studentId)
        {
            Test? test = await context.Tests
                                      .Where(t => !t.IsDeleted)
                                      .Include(t => t.ClassesWithAccess)
                                      .ThenInclude(ct => ct.Class)
                                      .ThenInclude(c => c.Students)
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

            if (test.PublicyLevel == PublicityLevel.ClassOnly)
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
            //TODO Admin test results
            var testQuery = context.Tests
                                   //.Include(t => t.TestResults)
                                   .Include(t => t.TestLikes)
                                   .Where(t => !t.IsDeleted);
            return await Filter(testQuery, query, null, null);
        }

        public List<TestGroupViewModel> GetGroupsByTest(Guid testId)
        {
            return context.TestGroups
                          .Where(g => g.TestId == testId)
                          .Select(g => new TestGroupViewModel()
                                       {
                                           AverageScore = g.TestResults.Average(r => r.Score),
                                           Examiners = g.TestResults.Count(),
                                           Id = g.Id,
                                           Number = g.Number
                                       })
                          .ToList();
        }
    }
}