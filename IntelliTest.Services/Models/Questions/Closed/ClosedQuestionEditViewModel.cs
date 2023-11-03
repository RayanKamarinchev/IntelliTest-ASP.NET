using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Questions.Closed
{
    public class ClosedQuestionEditViewModel : QuestionViewModel
    {
        public string[] Answers { get; set; }
        public bool[] AnswerIndexes { get; set; }
        public int MaxScore { get; set; }
        public IFormFile? Image { get; set; }
    }
}
