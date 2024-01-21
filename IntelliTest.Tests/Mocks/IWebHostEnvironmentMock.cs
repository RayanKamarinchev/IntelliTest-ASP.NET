using Microsoft.AspNetCore.Hosting;
using Moq;

namespace IntelliTest.Tests.Mocks
{
    public class IWebHostEnvironmentMock
    {
        public static IWebHostEnvironment Instance
        {
            get
            {
                var testServiceMock = new Mock<IWebHostEnvironment>();
                return testServiceMock.Object;
            }
        }
    }
}
