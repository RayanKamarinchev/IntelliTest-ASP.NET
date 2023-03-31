using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Questions
{
    public class OpenQuestionReviewViewModel : OpenQuestionAnswerViewModel
    {
        public string RightAnswer { get; set; }
        public decimal Score { get; set; }
    }
}
