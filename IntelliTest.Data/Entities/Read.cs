using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class Read
    {
        public Lesson Lesson { get; set; }
        [ForeignKey(nameof(Lesson))]
        public Guid LessonId { get; set; }

        public User User { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
    }
}
