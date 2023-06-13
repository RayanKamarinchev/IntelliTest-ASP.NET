using IntelliTest.Core.Contracts;
using Moq;
using IntelliTest.Data.Entities;

namespace IntelliTest.Tests.Mocks
{
    public class StudentsServiceMock
    {
        public static IStudentService Instance
        {
            get
            {
                var studentsServiceMock = new Mock<IStudentService>();
                Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
                studentsServiceMock.Setup(s => s.GetStudent(id))
                               .ReturnsAsync(new Student()
                               {
                                   Grade = 8,
                                   School = "PMG Sliven",
                                   Id = id,
                                   Classes = new List<StudentClass>()
                                   {
                                       new StudentClass()
                                       {
                                           Class = new Class()
                                           {
                                               Teacher = new Teacher()
                                               {
                                                   Id = id
                                               }
                                           }
                                       }
                                   }
                               });
                return studentsServiceMock.Object;
            }
        }
    }
}
