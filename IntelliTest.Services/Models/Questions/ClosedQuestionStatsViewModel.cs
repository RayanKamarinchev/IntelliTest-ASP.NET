using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Questions
{
    public class ClosedQuestionStatsViewModel
    {
        public string[] Answers { get; set; }
        public int Order { get; set; }
        public string Text { get; set; }
        public List<List<int>> StudentAnswers { get; set; }
    }
}
