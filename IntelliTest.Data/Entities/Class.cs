using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        public Teacher Teacher { get; set; }
        [ForeignKey(nameof(Teacher))]
        public int TeacherId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<StudentClass> Students { get; set; }
    }
}
