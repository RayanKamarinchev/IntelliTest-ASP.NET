using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IntelliTest.Data.Enums;

namespace IntelliTest.Data.Entities
{
    public class Test
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(30)]
        public string Title { get; set; }
        [Required]
        public Subject Subject { get; set; }
        [Required]
        [Range(0, 12)]
        public int Grade { get; set; }
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        public int Time { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }

        [Required] public IEnumerable<TestGroup> Groups { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public bool MultiSubmission { get; set; }
        [Required]
        public Teacher Creator { get; set; }
        [ForeignKey(nameof(Creator))]
        public Guid CreatorId { get; set; }
        public List<ClassTest> ClassesWithAccess { get; set; }
        public PublicityLevel PublicyLevel { get; set; }
        public string PhotoPath { get; set; }
        public IEnumerable<TestLike> TestLikes { get; set; }
        public IEnumerable<TestResult> TestResults { get; set; }
    }
}
