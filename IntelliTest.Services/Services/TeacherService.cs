using IntelliTest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using IntelliTest.Data.Entities;

namespace IntelliTest.Core.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IntelliTestDbContext context;

        public TeacherService(IntelliTestDbContext _context)
        {
            context = _context;
        }

        public async Task<bool> ExistsByUserId(string id)
        {
            return await context.Teachers.AnyAsync(x => x.UserId == id);
        }

        public async Task AddTeacher(string userId)
        {
            Teacher teacher = new Teacher()
            {
                UserId = userId
            };
            await context.Teachers.AddAsync(teacher);
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsTestCreator(Guid testId, Guid teacherId)
        {
            var teacher = await context.Teachers
                                       .Include(t => t.Tests)
                                       .FirstOrDefaultAsync(t => t.Id == teacherId);
            return teacher.Tests.Any(t => t.Id == testId);

        }

        public async Task<bool> IsLessonCreator(Guid lessonId, Guid teacherId)
        {
            var teacher = await context.Teachers
                                       .Include(t => t.Lessons)
                                       .FirstOrDefaultAsync(t => t.Id == teacherId);
            return teacher.Lessons.Any(t => t.Id == lessonId);
        }

        public async Task<Guid> GetTeacherId(string userId)
        {
            var teacher = await context.Teachers
                                       .Include(t => t.Tests)
                                       .FirstOrDefaultAsync(t => t.UserId == userId);
            return teacher.Id;
        }
    }
}
