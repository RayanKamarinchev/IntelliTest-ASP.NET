using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace IntelliTest.Core.Models.Questions
{
    public class OpenQuestionViewModel : QuestionViewModel
    {
        public string Answer { get; set; }
        public int MaxScore { get; set; }
    }
}
