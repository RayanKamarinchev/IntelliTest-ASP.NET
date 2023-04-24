using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Classes;
using IntelliTest.Core.Models.Users;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntelliTest.Core.Services
{
    public class ClassService : IClassService
    {
        private readonly IntelliTestDbContext context;

        public ClassService(IntelliTestDbContext _context)
        {
            context = _context;
        }
        public async Task<IEnumerable<ClassViewModel>> GetAll()
        {
            return await context.Classes
                                .Where(c => !c.IsDeleted)
                                .Select(c => new ClassViewModel()
                                {
                                    Description = c.Description,
                                    Name = c.Name,
                                    Id = c.Id,
                                    Teacher = new TeacherViewModel()
                                    {
                                        FullName = c.Teacher.User.FirstName + " " + c.Teacher.User.LastName,
                                        Id = c.TeacherId,
                                        School = c.Teacher.School
                                    },
                                    ImageUrl = c.ImageUrl,
                                    Subject = c.Subject
                                }).ToListAsync();
        }

        public async Task Create(ClassViewModel model)
        {
            Class dbClass = new Class()
            {
                Description = model.Description,
                Name = model.Name,
                TeacherId = model.Teacher.Id,
                Subject = model.Subject,
                ImageUrl = model.ImageUrl
            };
            await context.Classes.AddAsync(dbClass);
            await context.SaveChangesAsync();
        }
    }
}
