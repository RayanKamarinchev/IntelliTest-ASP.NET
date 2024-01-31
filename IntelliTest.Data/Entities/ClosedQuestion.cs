using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntelliTest.Data.Entities
{
    public class ClosedQuestion
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string AnswerIndexes { get; set; }
        [Required]
        public string Answers { get; set; }

        public bool IsDeleted { get; set; }
        [Required]
        public Guid GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public TestGroup TestGroup { get; set; }
        public IEnumerable<ClosedQuestionAnswer> StudentAnswers { get; set; }
        public float MaxScore { get; set; }
        public Guid? LessonId { get; set; }
        [ForeignKey(nameof(LessonId))]
        public Lesson? Lesson { get; set; }
        [Required]
        public bool IsEquation { get; set; }
        public string ImagePath { get; set; }
    }
}
