using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Data.Enums;

namespace IntelliTest.Core.Models
{
    public class Filter
    {
        public string SearchTerm { get; set; }
        public Sorting Sorting { get; set; }
        public Subject Subject { get; set; }
        public int Grade { get; set; }
    }
}
