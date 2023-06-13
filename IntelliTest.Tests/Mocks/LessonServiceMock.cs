using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliTest.Tests.Mocks
{
    public class LessonServiceMock
    {
        public static ILessonService Instance
        {
            get
            {
                var lessonServiceMock = new Mock<ILessonService>();
                Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
                lessonServiceMock.Setup(s => s.ExistsById(id, id))
                                 .ReturnsAsync(true);
                lessonServiceMock.Setup(s => s.IsLessonCreator(id, id))
                                 .ReturnsAsync(true);
                return lessonServiceMock.Object;
            }
        }
    }
}
