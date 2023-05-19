using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntelliTest.Data;
using IntelliTest.Data.Entities;
using IntelliTest.Data.Enums;
using IntelliTest.Tests.Mocks;
using NUnit.Framework;

namespace IntelliTest.Tests.Unit_Tests
{
    public class UnitTestBase
    {
        protected IntelliTestDbContext data;

        [OneTimeSetUp]
        public void SetUpBase()
        {
            data = DbMock.Instance;
            SeedDatabase();
        }

        [OneTimeTearDown]
        public void TearDownBase()
        {
            data.Dispose();
        }

        private void SeedDatabase()
        {
            var userTeacher = new User()
            {
                Email = "teacher@gmail.com",
                FirstName = "Antonio",
                LastName = "Vivaldi"
            };
            data.Users.Add(userTeacher);
            var userStudent = new User()
            {
                Email = "student@gmail.com",
                FirstName = "Pesho",
                LastName = "Peshov"
            };
            data.Users.Add(userStudent);
            var teacher = new Teacher()
            {
                User = userStudent,
                School = "PMG Sliven"
            };
            data.Teachers.Add(teacher);
            var student = new Student()
            {
                Grade = 8,
                School = "PMG Sliven",
                User = userStudent
                //Grades
            };
            data.Students.Add(student);
            var clas = new Class()
            {
                Description = "Test class",
                ImageUrl = "imgs/263578a3-3347-4965-a1cf-08be0d5f29dc_test-img-removebg-preview.png",
                Name = "Math class",
                Subject = Subject.Математика,
                Teacher = teacher
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
                PublicyLevel = PublicityLevel.ClassOnly
            };
            data.Tests.Add(test);
            var closedQuestions = new List<ClosedQuestion>()
            {
                new ClosedQuestion()
                {
                    Answers = "едно&две&три&четири",
                    AnswerIndexes = "1",
                    MaxScore = 2,
                    Order = 0,
                    Test = test,
                    Text = "Избери"
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
                    Order = 1
                },
                new OpenQuestion()
                {
                    Text = "How are you",
                    Answer = "Fine",
                    Test = test,
                    MaxScore = 1,
                    Order = 2
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
                    Question = closedQuestions[0]
                }
            };
            data.ClosedQuestionAnswers.AddRange(closedQuestionAnswers);
            var openQuestionAnswers = new List<OpenQuestionAnswer>()
            {
                new OpenQuestionAnswer()
                {
                    Answer = "its me mario",
                    Student = student,
                    Question = openQuestionjs[0]
                },
                new OpenQuestionAnswer()
                {
                    Answer = "Bad",
                    Student = student,
                    Question = openQuestionjs[1]
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
                Title = "Math lesson"
            };
            data.Lessons.Add(lesson);
            var reads = new List<Read>()
            {
                new Read()
                {
                    Lesson = lesson,
                    User = userStudent
                }
            };
            data.Reads.AddRange(reads);
            //Test result
        }
    }
}
