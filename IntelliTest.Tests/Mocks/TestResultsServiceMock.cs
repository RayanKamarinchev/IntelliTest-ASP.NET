using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models.Tests.Groups;
using Moq;

namespace IntelliTest.Tests.Mocks
{
    public class TestResultsServiceMock
    {
        public static ITestResultsService Instance
        {
            get
            {
                var testServiceMock = new Mock<ITestResultsService>();
                Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
                testServiceMock.Setup(s => s.ToEdit(It.IsAny<TestViewModel>()))
                               .Returns(new GroupEditViewModel());
                testServiceMock.Setup(s => s.TestsTakenByClass(id))
                               .ReturnsAsync(new List<TestGroupStatsViewModel>());
                return testServiceMock.Object;
            }
        }
    }
}
