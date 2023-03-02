using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class Student
    {
        public User User { get; set; }
        public int UserId { get; set; }
        public IEnumerable<Test> Tests { get; set; } = new List<Test>();
    }
}
