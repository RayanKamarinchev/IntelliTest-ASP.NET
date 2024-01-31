using IntelliTest.Core.Models.Tests.Groups;

namespace IntelliTest.Core.Models.Tests
{
    public class TestStatsViewModel
    {
        public string Title { get; set; }
        public float AverageScore { get; set; }
        public int Examiners { get; set; }
        public List<TestGroupStatsViewModel> TestGroups { get; set; }
    }
}
