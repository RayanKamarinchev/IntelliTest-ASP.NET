using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models.Questions;

namespace IntelliTest.Core.Models.Tests
{
    public class TestStatsViewModel
    {
        public List<ClosedQuestionStatsViewModel> ClosedQuestions { get; set; } =
            new List<ClosedQuestionStatsViewModel>();

        public List<OpenQuestionStatsViewModel> OpenQuestions { get; set; } =
            new List<OpenQuestionStatsViewModel>();
        public string Title { get; set; }
        public decimal AverageScore { get; set; }
        public int Examiners { get; set; }
    }
}
