using IntelliTest.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace IntelliTest.Models.Tests
{
    public class TestViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "Заглавие")]
        public string Title { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name = "Предмет")]
        public string Subject { get; set; }
        [Required]
        [Range(0, 12)]
        [Display(Name = "Клас")]
        public int Grade { get; set; }
        [Required]
        [StringLength(1000)]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [StringLength(50)]
        [Display(Name = "Училище")]
        public string School { get; set; }
        [Required]
        public decimal AverageScore { get; set; }
        [Required]
        public int MaxScore { get; set; }
        [Required]
        public int Students { get; set; }
        [Required]
        [Display(Name = "Време")]
        public int Time { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }
        public IList<OpenQuestion> OpenQuestions { get; set; }
        public IList<ClosedQuestion> ClosedQuestions { get; set; }
        public bool MultiSubmit { get; set; }
    }
}
