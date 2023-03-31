using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Data.Entities
{
    public class ClassTest
    {
        public Test Test { get; set; }
        [ForeignKey(nameof(Test))]
        public Guid TestId { get; set; }
        public Class Class { get; set; }
        [ForeignKey(nameof(Class))]
        public Guid ClassId { get; set; }
    }
}
