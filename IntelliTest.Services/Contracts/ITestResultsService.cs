using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Questions.Closed;
using IntelliTest.Core.Models.Questions.Open;
using IntelliTest.Core.Models.Tests;

namespace IntelliTest.Core.Contracts
{
    public interface ITestResultsService
    {
        //Answer processing
        Task AddTestAnswer(List<OpenQuestionSubmitViewModel> openQuestions,
                           List<ClosedQuestionViewModel> closedQuestions,
                           Guid studentId,
                           Guid testId);
        decimal CalculateClosedQuestionScore(bool[] Answers, int[] RightAnswers, int MaxScore);
        bool[] ProccessAnswerIndexes(string[] answers, string answerIndexes);
        TestEditViewModel ToEdit(TestViewModel model);
        //Statistics and results
        Guid[] GetExaminersIds(Guid testId);
        Task<TestStatsViewModel> GetStatistics(Guid testId);
        public Task<List<TestStatsViewModel>> TestsTakenByClass(Guid classId);
        Task<TestReviewViewModel> GetStudentsTestResults(Guid testId, Guid studentId);
        Task<IEnumerable<TestResultsViewModel>> GetStudentsTestsResults(Guid studentId);
        Task SubmitTestScore(Guid testId, Guid studentId, TestReviewViewModel scoredTest);
    }
}
