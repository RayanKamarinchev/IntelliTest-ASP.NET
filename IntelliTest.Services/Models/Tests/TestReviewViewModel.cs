using IntelliTest.Core.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models.Enums;
using IntelliTest.Core.Models.Questions.Closed;

namespace IntelliTest.Core.Models.Tests
{
    public class TestReviewViewModel
    {
        public List<ClosedQuestionReviewViewModel> ClosedQuestions { get; set; }
        public List<OpenQuestionReviewViewModel> OpenQuestions { get; set; }
        public List<QuestionType> QuestionOrder { get; set; }
        public decimal Score { get; set; }
    }
}
