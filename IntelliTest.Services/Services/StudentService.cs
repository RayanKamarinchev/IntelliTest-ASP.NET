using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models.Users;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntelliTest.Core.Services
{
    public class StudentService : IStudentService
    {
        private readonly IntelliTestDbContext context;

        public StudentService(IntelliTestDbContext _context)
        {
            context = _context;
        }

        public async Task AddStudent(UserType model, string userId)
        {
            Student student = new Student()
            {
                UserId = userId,
                Grade = model.Grade,
                School = model.School
            };
            await context.Students.AddAsync(student);
            await context.SaveChangesAsync();
        }

        public async Task<Guid?> GetStudentId(string userId)
        {
            var student = await context.Students.FirstOrDefaultAsync(u => u.UserId == userId);
            if (student == null)
            {
                return null;
            }
            return student.Id;
        }

        public async Task<Student> GetStudent(Guid studentId)
        {
            return await context.Students
                                .Include(s=>s.ClosedAnswers)
                                .ThenInclude(q => q.Question.Test)
                                .Include(s=>s.OpenAnswers)
                                .ThenInclude(q=>q.Question.Test)
                                .FirstOrDefaultAsync(s=>s.Id==studentId);
        }

        public async Task<IEnumerable<TestResultsViewModel>> GetAllResults(Guid studentId)
        {
            return await context.TestResults
                         .Where(t => t.StudentId == studentId)
                         .Select(t => new TestResultsViewModel()
                         {
                             TakenOn = t.TakenOn,
                             Title = t.Test.Title,
                             Description = t.Test.Description,
                             Grade = t.Test.Grade,
                             Mark = t.Mark,
                             Score = t.Score,
                             TestId = t.TestId
                         }).ToListAsync();
        }
    }
}