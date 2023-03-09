using IntelliTest.Core.Models.Questions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IntelliTest.Core.Models.Tests
{
    public class TestSubmitViewModel
    {
        public List<OpenQuestionAnswerViewModel> OpenQuestions { get; set; }
        public List<ClosedQuestionAnswerViewModel> ClosedQuestions { get; set; }
        [Display(Name = "Заглавие: ")]
        public string Title { get; set; }
        public int Time { get; set; }
    }
}
