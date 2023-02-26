using System.Linq.Expressions;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using IntelliTest.Models.Tests;
using IntelliTest.Core.Contracts;
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

        public static TestViewModel ToTestViewModel(Test t)
        {
            return new TestViewModel()
            {
                Id = t.Id,
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
            };
        }

        public async Task<IEnumerable<TestViewModel>> GetAll()
        {
            return await context.Tests.Where(t=>!t.IsDeleted)
                           .Select(t=> new TestViewModel()
                           {
                               Id = t.Id,
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
                           })
                           .ToListAsync();
        }

        public async Task<IEnumerable<TestViewModel>> GetMy(string userId)
        {
            return await context.Tests
                                .Where(t=>t.User.Id == userId)
                                .Select(y => ToTestViewModel(y))
                                .ToListAsync();
        }

        public async Task<TestViewModel> GetById(int id)
        {
            var test = await context.Tests.FindAsync();
            
            return ToTestViewModel(test);
        }
    }
}
