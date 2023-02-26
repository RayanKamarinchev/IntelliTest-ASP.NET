using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntelliTest.Data.Entities
{
    public class OpenQuestion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string Answer { get; set; }
        [Required]
        public int Order { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public int TestId { get; set; }
        [ForeignKey(nameof(TestId))]
        public Test Test { get; set; }
    }
}
