using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Classes;
using Moq;

namespace IntelliTest.Tests.Mocks
{
    public class ClassServiceMock
    {
        public static IClassService Instance
        {
            get
            {
                var classServiceMock = new Mock<IClassService>();
                Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
                classServiceMock.Setup(s => s.GetById(id))
                               .ReturnsAsync(new ClassViewModel());
                classServiceMock.Setup(s => s.IsClassOwner(id, It.IsAny<string>()))
                                .ReturnsAsync(true);
                return classServiceMock.Object;
            }
        }
    }
}
