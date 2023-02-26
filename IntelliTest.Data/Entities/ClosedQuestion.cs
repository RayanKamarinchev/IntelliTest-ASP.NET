using System.ComponentModel.DataAnnotations;

namespace IntelliTest.Data.Entities
{
    public class ClosedQuestion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public int AnswerIndex { get; set; }
        [Required]
        public string Answers { get; set; }
        [Required]
        public int Order { get; set; }

        public bool IsDeleted { get; set; }
    }
}
