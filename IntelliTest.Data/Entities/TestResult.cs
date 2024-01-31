using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using IntelliTest.Data.Enums;

namespace IntelliTest.Data.Entities
{
    public class TestResult
    {
        public Guid GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public TestGroup Group { get; set; }
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }
        public Student Student { get; set; }
        public Mark Mark { get; set; }
        public float Score { get; set; }
        public DateTime TakenOn { get; set; }
    }
}
