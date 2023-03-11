using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Questions
{
    public class ClosedQuestionReviewViewModel : ClosedQuestionAnswerViewModel
    {
        public int[] RightAnswers { get; set; }
    }
}
