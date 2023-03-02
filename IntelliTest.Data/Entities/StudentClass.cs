using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class StudentClass
    {
        public Class Class { get; set; }
        public int ClassId { get; set; }

        public Student Student { get; set; }
        public int StudentId { get; set; }
    }
}
