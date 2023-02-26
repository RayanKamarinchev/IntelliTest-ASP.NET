using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Core.Models.Tests
{
    public class TestEditViewModel
    {
        public List<OpenQuestionViewModel> OpenQuestions { get; set; }
        public List<ClosedQuestionViewModel> ClosedQuestions { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Grade { get; set; }
        public int Time { get; set; }
    }
}
