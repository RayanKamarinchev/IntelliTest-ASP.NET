using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Data.Entities;

namespace IntelliTest.Core.Contracts
{
    public interface ITestService
    {
        Task<QueryModel<TestViewModel>> GetAll(Guid? teacherId, Guid? studentId, QueryModel<TestViewModel> query);
        Task<QueryModel<TestViewModel>> GetMy(Guid? teacherId, Guid? studentId, QueryModel<TestViewModel> query);
        Task<TestViewModel> GetById(Guid id);
        Task<bool> ExistsbyId(Guid id);
        TestEditViewModel ToEdit(TestViewModel model);
        Task Edit(Guid id, TestEditViewModel model, Guid teacherId);
        TestSubmitViewModel ToSubmit(TestViewModel model);
        Task<TestReviewViewModel> TestResults(Guid testId, Guid studentId);
        Task<bool> IsTestTakenByStudentId(Guid testId, Guid studentId);
        Task<TestStatsViewModel> GetStatistics(Guid testId);
        Task AddTestAnswer(List<OpenQuestionAnswerViewModel> openQuestions,
                           List<ClosedQuestionAnswerViewModel> closedQuestions,
                           Guid studentId,
                           Guid testId);
        Guid[] GetExaminersIds(Guid testId);
        Task<QueryModel<TestViewModel>> TestsTakenByStudent(Guid studentId, QueryModel<TestViewModel> query);
        Task<Guid> Create(TestViewModel model, Guid teacherId, string[] classNames);
        decimal CalculateClosedQuestionScore(bool[] Answers, int[] RightAnswers, int MaxScore);

        Task<Tuple<decimal, string>> CalculateOpenQuestionScore(string Answer, string RightAnswer, int MaxScore);
        public Task<bool> StudentHasAccess(Guid testId, Guid studentId);
        public Task DeleteTest(Guid id);
        public Task<List<TestStatsViewModel>> TestsTakenByClass(Guid classId);
        string Translate(string text);
        Task SaveChanges();
        bool[] ProccessAnswerIndexes(string[] answers, string answerIndexes);
        public Task<QueryModel<TestViewModel>> Filter(IQueryable<Test> testQuery, QueryModel<TestViewModel> query, Guid? teacherId, Guid? studentId);
        public Task<QueryModel<TestViewModel>> FilterMine(IEnumerable<Test> testQuery, QueryModel<TestViewModel> query);
        public Task<bool> IsTestCreator(Guid testId, Guid teacherId);
    }
}
