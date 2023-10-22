using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Questions
{
    public class OpenQuestionViewModel : QuestionViewModel
    {
        public string Answer { get; set; }
        public int MaxScore { get; set; }
        public string ImagePath { get; set; }
        public bool IsEquation { get; set; }
    }
}
