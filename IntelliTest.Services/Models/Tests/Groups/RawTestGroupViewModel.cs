using IntelliTest.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace IntelliTest.Core.Models.Tests.Groups
{
    public class RawTestGroupViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public int Number { get; set; }
        public string QuestionsOrder { get; set; }
        public List<OpenQuestion> OpenQuestions { get; set; } = new List<OpenQuestion>();
        public List<ClosedQuestion> ClosedQuestions { get; set; } = new List<ClosedQuestion>();
        public Guid TestId { get; set; }
        public int Time { get; set; }
        public string TestTitle { get; set; }
    }
}
