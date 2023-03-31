using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Data.Entities;
using IntelliTest.Models.Tests;

namespace IntelliTest.Core.Contracts
{
    public interface ITestService
    {
        Task<IEnumerable<TestViewModel>> GetAll();
        Task<IEnumerable<TestViewModel>> GetMy(int teacherId);
        Task<TestViewModel> GetById(int id);
        Task<bool> ExistsbyId(int id);
        TestEditViewModel ToEdit(TestViewModel model);
        Task Edit(int id, TestEditViewModel model, int teacherId);
        TestSubmitViewModel ToSubmit(TestViewModel model);
        Task<TestReviewViewModel> TestResults(int testId, int studentId);
        Task<bool> IsTestTakenByStudentId(int testId, Student student);
        Task<TestStatsViewModel> GetStatistics(int testId);
        Task AddTestAnswer(List<OpenQuestionAnswerViewModel> openQuestions,
                           List<ClosedQuestionAnswerViewModel> closedQuestions,
                           int studentId,
                           int testId);
        int[] GetStudentIds(int testId);
        Task<IEnumerable<TestViewModel>> TestsTakenByStudent(int studentId);
        Task<int> Create(TestViewModel model, int teacherId);
        decimal CalculateClosedQuestionScore(bool[] Answers, int[] RightAnswers, int MaxScore, int answersCount);

        decimal CalculateOpenQuestionScore(string Answer, string RightAnswer, int MaxScore);
    }
}
