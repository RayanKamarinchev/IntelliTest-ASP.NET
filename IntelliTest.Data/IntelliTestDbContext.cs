using IntelliTest.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IntelliTest.Data
{
    public class IntelliTestDbContext : IdentityDbContext
    {
        public IntelliTestDbContext(DbContextOptions<IntelliTestDbContext> options)
            : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<StudentClass>()
                   .HasKey(p=> new{p.ClassId, p.StudentId});

            FirstTeacher = new Teacher()
            {
                UserId = "e9242048-504d-4ea9-9776-47691844c4a6",
                Id = 1
            };
            builder.Entity<Teacher>()
                   .HasData(FirstTeacher);

            StudentClass = new StudentClass()
            {
                StudentId = 1,
                ClassId = 1
            };
            builder.Entity<StudentClass>()
                   .HasData(StudentClass);

            FirstStudent = new Student()
            {
                Grade = 8,
                Grades = "6&5",
                School = "PPMG Dobri Chintulov",
                UserId = "4fb46fcc-ad1d-4120-835d-d351849efc73",
                Id = 1
            };
            builder.Entity<Student>()
                   .HasData(FirstStudent);

            FirstClass = new Class()
            {
                Description = "This is the first class ever made",
                Name = "Nothing class",
                Id = 1,
                TeacherId = 1
            };
            builder.Entity<Class>()
                   .HasData(FirstClass);
            OpenQuestion = new OpenQuestion()
            {
                Answer = "Az",
                Id = 2,
                IsDeleted = false,
                Order = 0,
                Text = "Koi suzdade testut",
                TestId = 1,
                MaxScore = 3
            };
            builder.Entity<OpenQuestion>()
                   .HasData(OpenQuestion);
            FirstOpenQuestionAnswer = new OpenQuestionAnswer()
            {
                Answer = "Ti",
                StudentId = 1,
                Id = 1,
                QuestionId = 2
            };
            builder.Entity<OpenQuestionAnswer>()
                   .HasData(FirstOpenQuestionAnswer);
            ClosedQuestion = new ClosedQuestion()
            {
                AnswerIndexes = "1",
                Answers = "Ti&Az&dvamata&nikoi",
                Id = 1,
                IsDeleted = false,
                Order = 1,
                Text = "Koi suzdade testut",
                TestId = 1,
                MaxScore = 2
            };
            builder.Entity<ClosedQuestion>()
                   .HasData(ClosedQuestion);

            FirstClosedQuestionAnswer = new ClosedQuestionAnswer()
            {
                AnswerIndexes = "0",
                StudentId = 1,
                Id = 1,
                QuestionId = 1
            };
            builder.Entity<ClosedQuestionAnswer>()
                   .HasData(FirstClosedQuestionAnswer);

            FirstTest = new Test()
            {
                AverageScore = 67.5m,
                Title = "Електромагнитни вълни",
                CreatedOn = new DateTime(2023, 2, 26, 19, 53, 6, 58, DateTimeKind.Local).AddTicks(7307),
                Description = "Просто тест",
                Grade = 10,
                IsDeleted = false,
                School = "ППМГ Добри Чинтулов",
                Subject = "Физика",
                Time = 30,
                Id = 1,
                CreatorId = 1
            };
            builder.Entity<Test>()
                   .HasData(FirstTest);

            base.OnModelCreating(builder);
        }
        public Teacher FirstTeacher { get; set; }
        public StudentClass StudentClass { get; set; }
        public Class FirstClass { get; set; }
        public Student FirstStudent { get; set; }
        public Test FirstTest { get; set; }
        public OpenQuestion OpenQuestion { get; set; }
        public ClosedQuestionAnswer FirstClosedQuestionAnswer { get; set; }
        public ClosedQuestion ClosedQuestion { get; set; }
        public OpenQuestionAnswer FirstOpenQuestionAnswer { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<ClosedQuestionAnswer> ClosedQuestionAnswers { get; set; }
        public DbSet<OpenQuestionAnswer> OpenQuestionAnswers { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<OpenQuestion> OpenQuestions { get; set; }
        public DbSet<ClosedQuestion> ClosedQuestions { get; set; }
        public DbSet<User> Users { get; set; }
    }
}