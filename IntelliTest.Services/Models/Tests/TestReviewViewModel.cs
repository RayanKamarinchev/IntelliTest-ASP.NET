using IntelliTest.Core.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Tests
{
    public class TestReviewViewModel
    {
        public List<ClosedQuestionReviewViewModel> ClosedQuestions { get; set; }
        public List<OpenQuestionReviewViewModel> OpenQuestions { get; set; }
    }
}
