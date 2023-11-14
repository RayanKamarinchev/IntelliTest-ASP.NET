using IntelliTest.Core.Contracts;
using IntelliTest.Core.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Models.Users;
using IntelliTest.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework.Internal;

namespace IntelliTest.Tests.Unit_Tests
{
    [TestFixture]
    public class StudentsServiceTests : UnitTestBase
    {
        private IStudentService studentService;
        private Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        private Guid id2 = Guid.Parse("fcda0a94-d7f6-4836-a093-f69066f177c7");

        [OneTimeSetUp]
        public void SetUp()
        {
            studentService = new StudentService(data);
        }

        [Test]
        public async Task AddStudent_Correct()
        {
            string userId = "StudentUser2";
            int grade = 8;
            string school = "BestSchoolEver";
            UserType userType = new UserType()
            {
                Grade = grade,
                IsStudent = true,
                School = school
            };
            await studentService.AddStudent(userType, userId);

            Student dbStudent = await data.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            Assert.IsNotNull(dbStudent);
            Assert.AreEqual(grade, dbStudent.Grade);
            Assert.AreEqual(school, dbStudent.School);
        }

        [Test]
        public async Task GetStudentId_Correct()
        {
            Guid? studentId = studentService.GetStudentId("StudentUser");

            Assert.AreEqual(id,studentId);
        }

        [Test]
        public async Task GetStudent_Correct()
        {
            Student studentDb = await data.Students.FindAsync(id);
            Student studentTest = await studentService.GetStudent(id);

            string json1 = JsonConvert.SerializeObject(studentDb, Formatting.Indented,
                                                       new JsonSerializerSettings()
                                                       {
                                                           ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling
                                                                                             .Ignore
                                                       });
            string json2 = JsonConvert.SerializeObject(studentTest, Formatting.Indented,
                                                       new JsonSerializerSettings()
                                                       {
                                                           ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                       });
            Assert.AreEqual(json1, json2);
        }

        [Test]
        public async Task getClassStudents_Correct()
        {
            var clasDb = await data.Classes
                                      .Include(c => c.Students)
                                      .ThenInclude(s => s.Student)
                                      .ThenInclude(s => s.User)
                                      .Include(s => s.Students)
                                      .ThenInclude(s => s.Student)
                                      .ThenInclude(s => s.TestResults)
                                      .FirstOrDefaultAsync(c => c.Id == id);
            var expectedStudents = clasDb.Students
                                         .Select(s=>s.Student)
                         .Select(s => new StudentViewModel()
                         {
                             Name = s.User.FirstName + " " + s.User.LastName,
                             Email = s.User.Email,
                             Id = s.Id,
                             ImagePath = s.User.PhotoPath,
                             TestResults = new List<decimal>(){0.0m}
                         })
                         .ToList();
            var studentsTest = await studentService.getClassStudents(id);

            string json1 = JsonConvert.SerializeObject(expectedStudents, Formatting.Indented,
                                                       new JsonSerializerSettings()
                                                       {
                                                           ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling
                                                                                             .Ignore
                                                       });
            string json2 = JsonConvert.SerializeObject(studentsTest, Formatting.Indented,
                                                       new JsonSerializerSettings()
                                                       {
                                                           ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                                                       });
            Assert.AreEqual(json1, json2);
        }

        [Test]
        public async Task GetExaminers_Correct()
        {
            var students = await studentService.GetExaminers(id);
            Assert.AreEqual(1, students.Count());
        }
    }
}
