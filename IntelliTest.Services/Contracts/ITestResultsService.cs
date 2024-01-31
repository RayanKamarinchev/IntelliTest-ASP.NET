using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Questions.Closed;
using IntelliTest.Core.Models.Questions.Open;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models.Tests.Groups;

namespace IntelliTest.Core.Contracts
{
    public interface ITestResultsService
    {
        //Answer processing
        Task SaveStudentTestAnswer(List<OpenQuestionSubmitViewModel> openQuestions,
                           List<ClosedQuestionViewModel> closedQuestions,
                           Guid studentId,
                           Guid testId);
        float CalculateClosedQuestionScore(bool[] Answers, int[] RightAnswers, float MaxScore);
        bool[] ProccessAnswerIndexes(string[] answers, string answerIndexes);

        GroupEditViewModel ToEdit(TestViewModel test, RawTestGroupViewModel group);
        //Statistics and results
        Guid[] GetExaminersIds(Guid testId);
        Task<TestStatsViewModel> GetStatistics(Guid testId);
        public Task<List<TestStatsViewModel>> TestsTakenByClass(Guid classId);
        Task<TestReviewViewModel> GetStudentTestResults(Guid testId, Guid studentId);
        Task<IEnumerable<TestResultsViewModel>> GetStudentsTestsResults(Guid studentId);
        Task SubmitTestScore(Guid testId, Guid studentId, TestReviewViewModel scoredTest);
    }
}
