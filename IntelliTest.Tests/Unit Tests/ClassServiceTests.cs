using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Core.Contracts;
using IntelliTest.Core.Models.Tests;
using IntelliTest.Core.Models;
using IntelliTest.Core.Models.Classes;
using IntelliTest.Core.Models.Users;
using IntelliTest.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;
using Newtonsoft.Json;

namespace IntelliTest.Tests.Unit_Tests
{
    [TestFixture]
    public class ClassServiceTests : UnitTestBase
    {
        private IClassService classService;
        private Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
        private Guid id2 = Guid.Parse("fcda0a94-d7f6-4836-a093-f69066f177c7");

        [OneTimeSetUp]
        public void SetUp() =>
            classService = new ClassService(data);

        [Test]
        public async Task GetAll_Correct()
        {
            var firstClass = (await classService.GetAll("StudentUser", true, false)).FirstOrDefault();
            Assert.AreEqual(id, firstClass.Id);
        }

        [Test]
        public async Task GetById_Correct()
        {
            var expected = new ClassViewModel()
            {
                Description = "Test class",
                ImageUrl = "imgs/263578a3-3347-4965-a1cf-08be0d5f29dc_test-img-removebg-preview.png",
                Name = "Math class",
                Subject = Subject.Математика,
                Teacher = new TeacherViewModel()
                {
                    School = "PMG Sliven",
                    Id = id,
                    FullName = "Antonio Vivaldi",
                    ImageUrl = ""
                },
                Id = id
            };
            var received = await classService.GetById(id);
            Assert.AreEqual(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(expected));
        }

        [Test]
        public async Task IsClassOwner_Correct()
        {
            Assert.IsTrue(await classService.IsClassOwner(id, "TeacherUser"));
        }

        [Test]
        public async Task Create_Correct()
        {
            int oldClassCount = data.Classes.Count();
            await classService.Create(new ClassViewModel()
            {
                Description = "Test class 2",
                ImageUrl = "",
                Name = "Bulgarian class",
                Subject = Subject.Български,
                Teacher = new TeacherViewModel()
                {
                    Id = id,
                },
                Id = id2
            });
            Assert.AreEqual(oldClassCount+1, data.Classes.Count());
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task Edit_Correct()
        {
            var clas = new ClassViewModel()
            {
                Description = "Test class 2",
                ImageUrl = "",
                Name = "Bulgarian class",
                Subject = Subject.Български,
            };
            await classService.Edit(clas, id);
            var clasDb = await data.Classes.FirstOrDefaultAsync();
            Assert.AreEqual("", clasDb.ImageUrl);
            Assert.AreEqual("Test class 2", clasDb.Description);
            Assert.AreEqual("Bulgarian class", clasDb.Name);
            Assert.AreEqual(Subject.Български, clasDb.Subject);
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task Delete_Correct()
        {
            int oldClassCount = data.Classes.Count();
            await classService.Delete(id);
            Assert.AreEqual(oldClassCount - 1, data.Classes.Count());
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task getClassStudents_Correct()
        {
            var firstClassStudent = (await classService.getClassStudents(id)).FirstOrDefault();
            var expectedStudent = new StudentViewModel()
            {
                Name = "Pesho Peshov",
                Email = "student@gmail.com",
                Id = id,
                TestResults = new List<decimal>()
            };
            Assert.AreEqual(JsonConvert.SerializeObject(expectedStudent), JsonConvert.SerializeObject(firstClassStudent));
        }
        [Test]
        public async Task IsInClass_Correct()
        {
            Assert.IsTrue(await classService.IsInClass(id, "StudentUser", true, false));
        }
        [Test]
        public async Task RemoveStudent_Correct()
        {
            int oldClassStudentsCount = data.Classes.FirstOrDefault().Students.Count();
            await classService.RemoveStudent(id, id);
            Assert.AreEqual(oldClassStudentsCount-1, data.Classes.FirstOrDefault().Students.Count());
            SetUpBase();
            SetUp();
        }
        [Test]
        public async Task AddStudent_Correct()
        {
            int oldClassStudentsCount = data.Classes.FirstOrDefault().Students.Count();
            var newStudent = new Student()
            {
                Grade = 10,
                School = "PMG Sliven",
                UserId = "BestUser",
                Id = id2
            };
            data.Students.Add(newStudent);
            data.Classes.FirstOrDefault().Students.Add(new StudentClass()
            {
                StudentId = id2
            });
            await classService.AddStudent(id2, id);
            Assert.AreEqual(oldClassStudentsCount + 1, data.Classes.FirstOrDefault().Students.Count());
            SetUpBase();
            SetUp();
        }
    }
}
