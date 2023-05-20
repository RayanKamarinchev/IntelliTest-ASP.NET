using System.ComponentModel.DataAnnotations;
using IntelliTest.Core.Models.Classes;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;

namespace IntelliTest.Core.Models.Tests
{
    public class TestViewModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [StringLength(30,MinimumLength = 5)]
        [Display(Name = "Заглавие")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Предмет")]
        public Subject Subject { get; set; }
        [Required]
        [Range(1, 12)]
        [Display(Name = "Клас")]
        public int Grade { get; set; }
        [Required]
        [Display(Name = "Публичност")]
        public PublicityLevel PublicityLevel { get; set; }
        [Required]
        [StringLength(1000)]
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [StringLength(50, MinimumLength = 10)]
        [Display(Name = "Училище")]
        public string School { get; set; }
        public decimal AverageScore { get; set; }
        public int MaxScore { get; set; }
        public int Students { get; set; }
        [Required]
        [Display(Name = "Време (в минути)")]
        public int Time { get; set; }
        public DateTime CreatedOn { get; set; }
        public IList<OpenQuestion>? OpenQuestions { get; set; }
        public IList<ClosedQuestion>? ClosedQuestions { get; set; }
        public IList<bool> Selected { get; set; }
        public bool MultiSubmit { get; set; }
        public string PhotoPath { get; set; }
    }
}
