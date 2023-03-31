using IntelliTest.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Lessons
{
    public class EditLessonViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public int Grade { get; set; }
        public string Subject { get; set; }
        public string School { get; set; }
        public IEnumerable<OpenQuestion> OpenQuestions { get; set; }
        public IEnumerable<ClosedQuestion> ClosedQuestions { get; set; }
    }
}
