using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions.Closed;

namespace IntelliTest.Core.Models.Tests
{
    public class TestReviewViewModel
    {
        public List<ClosedQuestionReviewViewModel> ClosedQuestions { get; set; }
        public List<OpenQuestionReviewViewModel> OpenQuestions { get; set; }
        public List<QuestionType> QuestionOrder { get; set; }
        public float Score { get; set; }
    }
}
