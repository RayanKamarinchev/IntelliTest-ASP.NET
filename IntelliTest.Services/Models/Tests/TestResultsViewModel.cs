using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Models.Tests;

namespace IntelliTest.Core.Models.Tests
{
    public class TestResultsViewModel : TestViewModel
    {
        public int StudentId { get; set; }
        public decimal Score { get; set; }
    }
}
