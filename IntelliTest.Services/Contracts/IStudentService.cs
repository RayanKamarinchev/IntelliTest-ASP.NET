using IntelliTest.Core.Models.Users;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Data.Entities;
using IntelliTest.Core.Models.Tests;

namespace IntelliTest.Core.Contracts
{
    public interface IStudentService
    {
        Task<bool> ExistsByUserId(string id);
        Task AddStudent(UserType model, string userId);
        Task<int> GetStudentId(string userId);

        Task<Student> GetStudent(int studentId);
        Task<IEnumerable<TestResultsViewModel>> GetAllResults(int studentId);
    }
}
