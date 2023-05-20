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
        [Key]
        public Guid Id { get; set; }
        public string? AnswerIndexes { get; set; }
        public ClosedQuestion Question { get; set; }
        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }
        public Student Student { get; set; }
        [ForeignKey(nameof(Student))]
        public Guid StudentId { get; set; }
        public decimal Points { get; set; }
        public string? Explanation { get; set; }
    }
}
