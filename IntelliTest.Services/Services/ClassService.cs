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
        public async Task<IEnumerable<ClassViewModel>> GetAll(string userId, bool isStudent, bool isTeacher)
        {
            var query = context.Classes
                               .Include(c=>c.Students)
                               .Include(c=>c.Teacher)
                               .Where(c => !c.IsDeleted);
            if (isStudent)
            {
                query = query.Where(c => c.Students.Any(s => s.Student.UserId == userId));
            }

            if (isTeacher)
            {
                query = query.Where(c => c.Teacher.UserId == userId);
            }

            return await query.Select(c => new ClassViewModel()
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

        public async Task<ClassViewModel?> GetById(Guid id)
        {
            var c = await context.Classes
                                 .Include(c=>c.Teacher)
                                 .ThenInclude(t=>t.User)
                                 .FirstOrDefaultAsync(c=>c.Id == id);
            return new ClassViewModel()
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
            };
        }

        public async Task<bool> IsClassOwner(Guid id, string userId)
        {
            var c = await context.Classes
                                 .Include(c=>c.Teacher)
                                 .FirstOrDefaultAsync(c=>c.Id==id);
            return c.Teacher.UserId == userId;
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

        public async Task Edit(ClassViewModel model, Guid id)
        {
            var c = await context.Classes.FindAsync(id);
            c.Description = model.Description;
            c.Name = model.Name;
            c.Subject = model.Subject;
            await context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var c = await context.Classes.FindAsync(id);
            context.Classes.Remove(c);
            await context.SaveChangesAsync();
        }

        public async Task<List<StudentViewModel>> getClassStudents(Guid id)
        {
            var clasDb = await context.Classes
                                      .Include(c => c.Students)
                                      .ThenInclude(s => s.Student)
                                      .ThenInclude(s => s.User)
                                      .FirstOrDefaultAsync(c => c.Id == id);
            return clasDb.Students.Select(s => new StudentViewModel()
            {
                Name = s.Student.User.FirstName + " " + s.Student.User.LastName,
                Email = s.Student.User.Email,
                TestResults = s.Student.TestResults
                               .Where(t=>clasDb.ClassTests.Any(ct=>ct.TestId == t.TestId))
                               .Select(t=>t.Score)
                               .ToList()
            }).ToList();
        }

        public async Task<bool> IsInClass(Guid classId, string userId, bool isStudent, bool isTeacher)
        {
            if (isStudent)
            {
                var clasDb = await context.Classes
                                    .Include(c => c.Students)
                                    .ThenInclude(s=>s.Student)
                                    .FirstOrDefaultAsync(c => c.Id == classId);
                return clasDb.Students.Any(s => s.Student.UserId == userId);
            }

            if (isTeacher)
            {
                return await IsClassOwner(classId, userId);
            }
            //if admin in future
            return true;
        }

        public async Task<bool> RemoveStudent(Guid studentId, Guid id)
        {
            var clasDb = await context.Classes
                                      .Include(c => c.Students)
                                      .ThenInclude(s => s.Student)
                                      .FirstOrDefaultAsync(c => c.Id == id);
            if (clasDb==null)
            {
                return false;
            }

            var student = clasDb.Students.FirstOrDefault(s => s.StudentId == studentId);
            if (student == null)
            {
                return false;
            }
            return clasDb.Students.Remove(student);
        }

        public async Task<bool> AddStudent(Guid studentId, Guid id)
        {
            var clasDb = await context.Classes
                                     .Include(c => c.Students)
                                     .ThenInclude(s => s.Student)
                                     .FirstOrDefaultAsync(c => c.Id == id);
            if (clasDb == null)
            {
                return false;
            }

            if (!context.Students.Any(s=>s.Id==studentId))
            {
                return false;
            }
            
            clasDb.Students.Add(new StudentClass()
            {
                StudentId = studentId
            });
            return true;
        }
    }
}
