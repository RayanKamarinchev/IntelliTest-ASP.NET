﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Questions.Closed
{
    public class ClosedQuestionReviewViewModel : ClosedQuestionAnswerViewModel
    {
        public int[] RightAnswers { get; set; }
        public decimal Score { get; set; }
    }
}