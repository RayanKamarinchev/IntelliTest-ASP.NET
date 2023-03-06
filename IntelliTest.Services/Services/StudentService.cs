using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
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
            return context.Students.FirstOrDefaultAsync(u => u.UserId == userId).Id;
        }
    }
}
