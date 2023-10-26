using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Questions.Closed;
using System.ComponentModel.DataAnnotations;

namespace IntelliTest.Core.Models.Tests
{
    public class TestSubmitViewModel
    {
        public List<OpenQuestionAnswerViewModel> OpenQuestions { get; set; }
        public List<ClosedQuestionAnswerViewModel> ClosedQuestions { get; set; }
        public List<QuestionType> QuestionOrder;
        [Display(Name = "Заглавие: ")]
        public string Title { get; set; }
        public int Time { get; set; }
        public Guid Id { get; set; }
    }
}
