using IntelliTest.Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models.Questions;

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
    }
}
