using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntelliTest.Data.Entities
{
    public class TestGroup
    {
        [Key]
        public Guid Id { get; set; }
        [Required] 
        public int Number { get; set; }
        public string QuestionsOrder { get; set; }
        public IList<OpenQuestion> OpenQuestions { get; set; } = new List<OpenQuestion>();
        public IList<ClosedQuestion> ClosedQuestions { get; set; } = new List<ClosedQuestion>();
        [ForeignKey(nameof(Test))]
        public Guid TestId { get; set; }
        public Test Test { get; set; }
        public bool IsDeleted { get; set; }
        public IEnumerable<TestResult> TestResults { get; set; }
    }
}
