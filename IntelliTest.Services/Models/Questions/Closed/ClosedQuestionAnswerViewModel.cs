using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Questions.Closed
{
    public class ClosedQuestionAnswerViewModel : QuestionViewModel
    {
        public string[] PossibleAnswers { get; set; }
        public bool[] Answers { get; set; }
        public int MaxScore { get; set; }
    }
}
