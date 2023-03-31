using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class Teacher
    {
        [Key]
        public Guid Id { get; set; }
        public User User { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public IEnumerable<Class> Classes { get; set; }
        public IEnumerable<Test> Tests { get; set; }
        public IEnumerable<Lesson> Lessons { get; set; }
    }
}
