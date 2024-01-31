using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models.Users;

namespace IntelliTest.Core.Models.Classes
{
    public class ClassDetailsModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<StudentViewModel> Students { get; set; }
        public List<TestStatsViewModel> Tests { get; set; }
        public string JoinCode { get; set; }
    }
}
