using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Questions.Closed;

namespace IntelliTest.Core.Models.Tests
{
    public class TestStatsViewModel
    {
        public List<ClosedQuestionStatsViewModel> ClosedQuestions { get; set; } =
            new List<ClosedQuestionStatsViewModel>();

        public List<OpenQuestionStatsViewModel> OpenQuestions { get; set; } =
            new List<OpenQuestionStatsViewModel>();

        public List<QuestionType> QuestionOrder;
        public string Title { get; set; }
        public decimal AverageScore { get; set; }
        public int Examiners { get; set; }
    }
}
