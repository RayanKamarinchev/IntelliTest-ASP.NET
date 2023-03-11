using IntelliTest.Core.Models.Users;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Data.Entities;

namespace IntelliTest.Core.Contracts
{
    public interface IStudentService
    {
        Task<bool> ExistsByUserId(string id);
        Task AddStudent(UserType model, string userId);
        Task<int> GetStudentId(string userId);

        Task AddTestAnswer(List<OpenQuestionAnswerViewModel> openQuestions,
                           List<ClosedQuestionAnswerViewModel> closedQuestions,
                           string userId,
                           int testId);

        Task<Student> GetStudent(int studentId);
    }
}
