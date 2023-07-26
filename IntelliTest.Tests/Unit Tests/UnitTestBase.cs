using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;
using IntelliTest.Tests.Mocks;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace IntelliTest.Tests.Unit_Tests
{
    public class UnitTestBase
    {
        protected IntelliTestDbContext data;
        protected IConfiguration configuration;

        [OneTimeSetUp]
        public void SetUpBase()
        {
            data = DbMock.Instance;
            SeedDatabase();
            var inMemorySettings = new Dictionary<string, string> {
                {"TopLevelKey", "TopLevelValue"},
                {"SectionName:SomeKey", "SectionValue"}
            };

            configuration = new ConfigurationBuilder()
                                           .AddInMemoryCollection(inMemorySettings)
                                           .Build();
        }

        [OneTimeTearDown]
        public void TearDownBase()
        {
            data.Dispose();
        }

        private void SeedDatabase()
        {
            Guid id = Guid.Parse("c0b0d11d-cf99-4a2e-81a9-225d0b0c4e87");
            Guid id2 = Guid.Parse("fcda0a94-d7f6-4836-a093-f69066f177c7");
            var userTeacher = new User()
            {
                Email = "teacher@gmail.com",
                FirstName = "Antonio",
                LastName = "Vivaldi",
                Id = "TeacherUser",
                PhotoPath = ""
            };
            data.Users.Add(userTeacher);
            var userStudent = new User()
            {
                Email = "student@gmail.com",
                FirstName = "Pesho",
                LastName = "Peshov",
                Id = "StudentUser",
                PhotoPath = ""
            };
            data.Users.Add(userStudent);
            var bonusUser = new User()
            {
                Email = "thebest@gmail.com",
                FirstName = "Gosho",
                LastName = "Jordanov",
                Id = "BestUser",
                PhotoPath = ""
            };
            data.Users.Add(userStudent);
            var teacher = new Teacher()
            {
                User = userTeacher,
                School = "PMG Sliven",
                Id = id
            };
            data.Teachers.Add(teacher);
            var student = new Student()
            {
                Grade = 8,
                School = "PMG Sliven",
                User = userStudent,
                Id = id
                //Grades
            };
            data.Students.Add(student);
            var clas = new Class()
            {
                Description = "Test class",
                ImageUrl = "imgs/263578a3-3347-4965-a1cf-08be0d5f29dc_test-img-removebg-preview.png",
                Name = "Math class",
                Subject = Subject.Математика,
                Teacher = teacher,
                Id = id
            };
            data.Classes.Add(clas);
            var studentClass = new StudentClass()
            {
                Class = clas,
                Student = student
            };
            data.StudentClasses.Add(studentClass);
            var test = new Test()
            {
                CreatedOn = new DateTime(2023, 5, 19, 10, 51, 0),
                Creator = teacher,
                Description = "Test Test",
                Grade = 8,
                Time = 10,
                Title = "The test",
                PublicyLevel = PublicityLevel.TeachersOnly,
                Id = id,
                PhotoPath = "",
                QuestionsOrder = "O|C|O"
            };
            data.Tests.Add(test);
            var closedQuestions = new List<ClosedQuestion>()
            {
                new ClosedQuestion()
                {
                    Answers = "едно&две&три&четири",
                    AnswerIndexes = "1",
                    MaxScore = 2,
                    Test = test,
                    Text = "Избери",
                    Id = id
                }
            };
            data.ClosedQuestions.AddRange(closedQuestions);
            var openQuestionjs = new List<OpenQuestion>()
            {
                new OpenQuestion()
                {
                    Text = "Who are you",
                    Answer = "Its me, Mario",
                    Test = test,
                    MaxScore = 3,
                    Id = id
                },
                new OpenQuestion()
                {
                    Text = "How are you",
                    Answer = "Fine",
                    Test = test,
                    MaxScore = 1,
                    Id = id2
                },
            };
            data.OpenQuestions.AddRange(openQuestionjs);
            var classtest = new ClassTest()
            {
                Class = clas,
                Test = test
            };
            data.ClassTests.Add(classtest);
            var closedQuestionAnswers = new List<ClosedQuestionAnswer>()
            {
                new ClosedQuestionAnswer()
                {
                    AnswerIndexes = "2",
                    Student = student,
                    Question = closedQuestions[0],
                    Id = id,
                    Explanation = "None"
                }
            };
            data.ClosedQuestionAnswers.AddRange(closedQuestionAnswers);
            var openQuestionAnswers = new List<OpenQuestionAnswer>()
            {
                new OpenQuestionAnswer()
                {
                    Answer = "its me mario",
                    Student = student,
                    Question = openQuestionjs[0],
                    Id = id,
                    Explanation = "None"
                },
                new OpenQuestionAnswer()
                {
                    Answer = "Bad",
                    Student = student,
                    Question = openQuestionjs[1],
                    Id = id2,
                    Explanation = "None"
                }
            };
            data.OpenQuestionAnswers.AddRange(openQuestionAnswers);
            var lesson = new Lesson()
            {
                Content = "Example nonsense text",
                CreatedOn = new DateTime(2023, 5, 19, 11, 3, 0),
                Creator = teacher,
                Grade = 8,
                HtmlCotnent = "Example nonsense text",
                Title = "Math lesson",
                Id = id,
                Subject = Subject.Математика
            };
            data.Lessons.Add(lesson);
            var room = new Room()
            {
               Name = "Class room",
               AdminId = "TeacherUser",
               Id = id
            };
            data.Rooms.Add(room);
            var roomUsers = new List<RoomUser>()
            {
                new RoomUser()
                {
                    Room = room,
                    UserId = "StudentUser"
                },
                new RoomUser()
                {
                    Room = room,
                    UserId = "TeacherUser"
                }
            };
            data.RoomUsers.AddRange(roomUsers);
            var messages = new List<Message>()
            {
                new Message()
                {
                    Room = room,
                    Content = "Hello",
                    Sender = userStudent,
                    Timestamp = new DateTime(2023, 5, 21, 2, 41, 0),
                    Id = id
                },
                new Message()
                {
                    Room = room,
                    Content = "Hello Student",
                    Sender = userTeacher,
                    Timestamp = new DateTime(2023, 5, 21, 2, 42, 0),
                    Id = id2
                }
            };
            data.Messages.AddRange(messages);
            var testResult = new TestResult()
            {
                Mark = Mark.Unmarked,
                Score = 0,
                StudentId = id,
                TestId = id,
                TakenOn = new DateTime(2023, 5, 21, 2, 42, 0)
            };
            data.TestResults.Add(testResult);
            //Test result
            data.SaveChanges();
        }
    }
}
