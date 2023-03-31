using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Data.Entities;
using IntelliTest.Models.Tests;

namespace IntelliTest.Core.Models.Tests
{
    public class TestResultsViewModel
    {
        public int TestId { get; set; }
        public DateTime TakenOn { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Grade { get; set; }
        public Grade Mark { get; set; }
        public int StudentId { get; set; }
        public decimal Score { get; set; }
    }
}
