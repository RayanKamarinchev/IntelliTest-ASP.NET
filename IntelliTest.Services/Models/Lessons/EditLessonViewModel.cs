using IntelliTest.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Data.Enums;

namespace IntelliTest.Core.Models.Lessons
{
    public class EditLessonViewModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }

        public string HtmlContent { get; set; }
        [Range(1,12, ErrorMessage = "Mark between 1 to 12")]
        public int Grade { get; set; }
        public Subject Subject { get; set; }
        public string School { get; set; }
        public IEnumerable<OpenQuestion>? OpenQuestions { get; set; }
        public IEnumerable<ClosedQuestion> ClosedQuestions { get; set; }
    }
}
