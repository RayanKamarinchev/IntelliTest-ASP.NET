using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Tests;
using Moq;

namespace IntelliTest.Tests.Mocks
{
    public class TestServiceMock
    {
        public static ITestService Instance
        {
            get
            {
                var testServiceMock = new Mock<ITestService>();
                Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
                testServiceMock.Setup(s => s.GetById(id))
                               .ReturnsAsync(new TestViewModel());
                testServiceMock.Setup(s => s.GetAll(id, id, new QueryModel<TestViewModel>()))
                               .ReturnsAsync(new QueryModel<TestViewModel>());
                testServiceMock.Setup(s => s.ExistsbyId(id))
                               .ReturnsAsync(true);
                return testServiceMock.Object;
            }
        }
    }
}
