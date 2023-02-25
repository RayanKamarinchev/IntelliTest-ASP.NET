using IntelliTest.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace IntelliTest.Models.Tests
{
    public class TestViewModel
    {
        [Required]
        [StringLength(30)]
        public string Title { get; set; }
        [Required]
        [StringLength(30)]
        public string Subject { get; set; }
        [Required]
        [Range(0, 12)]
        public int Grade { get; set; }
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(50)]
        public string School { get; set; }
        [Required]
        public decimal AverageScore { get; set; }
        [Required]
        public int MaxScore { get; set; }
        [Required]
        public int Students { get; set; }
        [Required]
        public int Time { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }
        [Required]
        public string Color1 { get; set; }
        [Required]
        public string Color2 { get; set; }
        public IList<OpenQuestion> OpenQuestions { get; set; }
        public IList<ClosedQuestion> ClosedQuestions { get; set; }
    }
}
