using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public Teacher Creator { get; set; }
        [ForeignKey(nameof(Creator))]
        public int CreatorId { get; set; }

        public int Readers { get; set; }
        public List<LessonLike>? LessonLikes { get; set; }
        public int Grade { get; set; }
        public string Subject { get; set; }
        public string School { get; set; }
        public IEnumerable<OpenQuestion>? OpenQuestions { get; set; }
        public IEnumerable<ClosedQuestion>? ClosedQuestions { get; set; }
    }
}
