using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Questions
{
    public class OpenQuestionStatsViewModel
    {
        public int Order { get; set; }
        public string Text { get; set; }
        public List<string> StudentAnswers { get; set; }
    }
}
