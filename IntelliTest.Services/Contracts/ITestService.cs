using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Data.Entities;
using IntelliTest.Models.Tests;

namespace IntelliTest.Core.Contracts
{
    public interface ITestService
    {
        Task<IEnumerable<TestViewModel>> GetAll();
        Task<IEnumerable<TestViewModel>> GetMy(Guid teacherId);
        Task<TestViewModel> GetById(Guid id);
        Task<bool> ExistsbyId(Guid id);
        TestEditViewModel ToEdit(TestViewModel model);
        Task Edit(Guid id, TestEditViewModel model, Guid teacherId);
        TestSubmitViewModel ToSubmit(TestViewModel model);
        Task<TestReviewViewModel> TestResults(Guid testId, Guid studentId);
        Task<bool> IsTestTakenByStudentId(Guid testId, Student student);
        Task<TestStatsViewModel> GetStatistics(Guid testId);
        Task AddTestAnswer(List<OpenQuestionAnswerViewModel> openQuestions,
                           List<ClosedQuestionAnswerViewModel> closedQuestions,
                           Guid studentId,
                           Guid testId);
        Guid[] GetStudentIds(Guid testId);
        Task<IEnumerable<TestViewModel>> TestsTakenByStudent(Guid studentId);
        Task<Guid> Create(TestViewModel model, Guid teacherId);
        decimal CalculateClosedQuestionScore(bool[] Answers, int[] RightAnswers, int MaxScore, int answersCount);

        decimal CalculateOpenQuestionScore(string Answer, string RightAnswer, int MaxScore);
    }
}
