using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Questions
{
    public class OpenQuestionAnswerViewModel : QuestionViewModel
    {
        public string Answer { get; set; }
        public int MaxScore { get; set; }
    }
}
