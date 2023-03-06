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
    }
}
