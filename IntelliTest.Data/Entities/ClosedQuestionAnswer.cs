using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class ClosedQuestionAnswer
    {
        [Required]
        public int AnswerIndex { get; set; }
        public ClosedQuestion Question { get; set; }
        [ForeignKey(nameof(Question))]
        public int QuestionId { get; set; }
        public Student Student { get; set; }
        [ForeignKey(nameof(Student))]
        public int StudentId { get; set; }
    }
}
