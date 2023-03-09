using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Questions
{
    public class ClosedQuestionViewModel : QuestionViewModel
    {
        public string[] Answers { get; set; }
        public bool[] AnswerIndexes { get; set; }
    }
}
