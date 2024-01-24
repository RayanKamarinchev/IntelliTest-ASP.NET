using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Questions.Closed;
using System.ComponentModel.DataAnnotations;
using IntelliTest.Core.Models.Questions.Open;

namespace IntelliTest.Core.Models.Tests
{
    public class TestSubmitViewModel
    {
        public List<OpenQuestionSubmitViewModel> OpenQuestions { get; set; }
        public List<ClosedQuestionViewModel> ClosedQuestions { get; set; }
        public List<QuestionType> QuestionOrder;
        [Display(Name = "Заглавие: ")]
        public string Title { get; set; }
        public int Time { get; set; }
        public Guid Id { get; set; }
    }
}
