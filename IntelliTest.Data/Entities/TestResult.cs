using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class TestResult
    {
        [ForeignKey(nameof(Test))]
        public int TestId { get; set; }
        public Test Test { get; set; }
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public Grade Grade { get; set; }
        public decimal Score { get; set; }
        public DateTime TakenOn { get; set; }
    }
}
