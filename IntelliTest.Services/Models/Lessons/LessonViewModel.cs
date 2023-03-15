using IntelliTest.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Lessons
{
    public class LessonViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public int CreatorId { get; set; }
        public int Readers { get; set; }
        public int Likes { get; set; }
        public int Grade { get; set; }
        public string Subject { get; set; }
        public string School { get; set; }
        public int ReadingTime { get; set; }
        public IEnumerable<OpenQuestion> OpenQuestions { get; set; }
        public IEnumerable<ClosedQuestion> ClosedQuestions { get; set; }
    }
}
