using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions;
using IntelliTest.Data.Enums;

namespace IntelliTest.Core.Models.Tests
{
    public class TestEditViewModel
    {
        public PublicityLevel PublicityLevel { get; set; }
        public List<OpenQuestionViewModel>? OpenQuestions { get; set; }
        public List<ClosedQuestionViewModel>? ClosedQuestions { get; set; }
        public List<QuestionType>? QuestionsOrder { get; set; }
        [Display(Name = "Заглавие: ")]
        public string Title { get; set; }
        [Display(Name = "Описание: ")]
        public string Description { get; set; }
        [Display(Name = "Клас: ")]
        public int Grade { get; set; }
        [Display(Name = "Време (мин): ")]
        public int Time { get; set; }
        public Guid? Id { get; set; }
    }
}
