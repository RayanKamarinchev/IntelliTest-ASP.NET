using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Questions;
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

        public async Task<bool> ExistsByUserId(string id)
        {
            return await context.Students.AnyAsync(x => x.UserId == id);
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

        public async Task<int> GetStudentId(string userId)
        {
            var student = await context.Students.FirstOrDefaultAsync(u => u.UserId == userId);
            return student.Id;
        }

        public async Task AddTestAnswer(List<OpenQuestionAnswerViewModel> openQuestions,
                                        List<ClosedQuestionAnswerViewModel> closedQuestions, string userId, int testId)
        {
            int studentId = await GetStudentId(userId);
            var open = openQuestions.Select(q => new OpenQuestionAnswer()
            {
                Answer = q.Answer,
                QuestionId = q.Id,
                StudentId = studentId
            });
            var closed = closedQuestions.Select(q => new ClosedQuestionAnswer()
            {
                AnswerIndexes = string.Join("&", q.Answers
                                                 .Select((val, indx) => new { val, indx })
                                                 .Where(q => q.val)
                                                 .Select(q => q.indx)),
                QuestionId = q.Id,
                StudentId = studentId
            });
            context.OpenQuestionAnswers.AddRange(open);
            context.ClosedQuestionAnswers.AddRange(closed);
            await context.SaveChangesAsync();
        }

        public async Task<Student> GetStudent(int studentId)
        {
            return await context.Students
                                .Include(s=>s.ClosedAnswers)
                                .ThenInclude(q => q.Question.Test)
                                .Include(s=>s.OpenAnswers)
                                .ThenInclude(q=>q.Question.Test)
                                .FirstOrDefaultAsync(s=>s.Id==studentId);
        }
    }
}