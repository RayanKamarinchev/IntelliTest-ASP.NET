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

        private static Func<Student, Class?, StudentViewModel> CreateViewModel = (student, classDb) =>
        {
            var viewModel = new StudentViewModel()
            {
                Name = student.User.FirstName + " " + student.User.LastName,
                Email = student.User.Email,
                Id = student.Id,
                ImagePath = student.User.PhotoPath
            };

            if (classDb != null)
            {
                viewModel.TestResults = student.TestResults
                                               .Where(t => classDb.ClassTests.Any(ct => ct.TestId == t.TestId))
                                               .Select(t => t.Score)
                                               .ToList();
            }
            else
            {
                viewModel.TestResults = student.TestResults
                                               .Select(t => t.Score)
                                               .ToList();
            }

            return viewModel;
        };
        
        private Func<Student, Class, StudentViewModel> ToViewModelWithClass = (s, classDb) => CreateViewModel(s, classDb);
        
        private Func<Student, StudentViewModel> ToViewModel = s => CreateViewModel(s, null);

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

        public Guid? GetStudentId(string userId)
        {
            var student = context.Students.FirstOrDefault(u => u.UserId == userId);
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
                                .Include(s=>s.Classes)
                                .ThenInclude(c=>c.Class)
                                .ThenInclude(c=>c.Teacher)
                                .FirstOrDefaultAsync(s=>s.Id==studentId);
        }

        public async Task<List<StudentViewModel>> getClassStudents(Guid id)
        {
            var clasDb = await context.Classes
                                      .Include(c => c.Students)
                                      .ThenInclude(s => s.Student)
                                      .ThenInclude(s => s.User)
                                      .Include(s => s.Students)
                                      .ThenInclude(s => s.Student)
                                      .ThenInclude(s => s.TestResults)
                                      .FirstOrDefaultAsync(c => c.Id == id);
            return clasDb.Students
                         .Select(s => ToViewModelWithClass(s.Student, clasDb))
                         .ToList();
        }

        public async Task<IEnumerable<StudentViewModel>> GetExaminers(Guid testId)
        {
            return await context.TestResults
                                .Include(t => t.Student)
                                .ThenInclude(s=>s.User)
                                .Include(t=>t.Student)
                                .ThenInclude(s=>s.TestResults)
                                .Where(t => t.TestId == testId)
                                .Select(s => ToViewModel(s.Student))
                                .ToListAsync();
        }
    }
}