using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntelliTest.Data.Entities
{
    public class OpenQuestion
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string Answer { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public Guid TestId { get; set; }
        [ForeignKey(nameof(TestId))]
        public Test Test { get; set; }

        public IEnumerable<OpenQuestionAnswer> StudentAnswers { get; set; }
        public int MaxScore { get; set; }
        public Guid? LessonId { get; set; }
        [ForeignKey(nameof(LessonId))]
        public Lesson? Lesson { get; set; }
        [Required]
        public bool IsEquation { get; set; }

        public string ImagePath { get; set; }
    }
}
