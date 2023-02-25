using IntelliTest.Contracts;
using IntelliTest.Data;
using IntelliTest.Models.Tests;
using Microsoft.EntityFrameworkCore;

namespace IntelliTest.Services
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
            return  await context.Tests.Where(t=>!t.IsDeleted)
                                 .Select(t=> new TestViewModel()
                                 {
                                     School = t.School,
                                     Time = t.Time,
                                     Title = t.Title,
                                     AverageScore = t.AverageScore,
                                     ClosedQuestions = t.ClosedQuestions,
                                     Color1 = t.Color1,
                                     Color2 = t.Color2,
                                     CreatedOn = t.CreatedOn,
                                     Description = t.Description,
                                     Grade = t.Grade,
                                     MaxScore = t.MaxScore,
                                     OpenQuestions = t.OpenQuestions
                                 }).ToListAsync();
        }

        public async Task<IEnumerable<TestViewModel>> GetMy(string userId)
        {
            return await context.Tests
                                .Where(t=>t.User.Id == userId)
                                .Select(t => new TestViewModel()
                                {
                                    School = t.School,
                                    Time = t.Time,
                                    Title = t.Title,
                                    AverageScore = t.AverageScore,
                                    ClosedQuestions = t.ClosedQuestions,
                                    Color1 = t.Color1,
                                    Color2 = t.Color2,
                                    CreatedOn = t.CreatedOn,
                                    Description = t.Description,
                                    Grade = t.Grade,
                                    MaxScore = t.MaxScore,
                                    OpenQuestions = t.OpenQuestions
                                }).ToListAsync();
        }
    }
}
