using System.ComponentModel.DataAnnotations;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions.Closed;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Core.Models.Tests.Groups;
using IntelliTest.Data.Enums;

namespace IntelliTest.Core.Models.Tests
{
    public class TestGroupEditViewModel
    {
        public List<TestGroupViewModel> Groups { get; set; }
        public Guid GroupId { get; set; }
        public PublicityLevel PublicityLevel { get; set; }
        public List<OpenQuestionViewModel>? OpenQuestions { get; set; }
        public List<ClosedQuestionViewModel>? ClosedQuestions { get; set; }
        public List<QuestionType>? QuestionsOrder { get; set; }
        [Display(Name = "Заглавие: ")]
        public string Title { get; set; }
        [Display(Name = "Описание: ")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }
        [Display(Name = "Клас: ")]
        public int Grade { get; set; }
        [Display(Name = "Време (мин): ")]
        public int Time { get; set; }
        public Guid? Id { get; set; }
    }
}
