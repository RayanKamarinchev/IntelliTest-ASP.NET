using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Data.Enums;

namespace IntelliTest.Core.Models.Tests
{
    public class TestResultsViewModel
    {
        public Guid TestId { get; set; }
        public DateTime TakenOn { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Grade { get; set; }
        public Mark Mark { get; set; }
        public Guid StudentId { get; set; }
        public float Score { get; set; }
    }
}
