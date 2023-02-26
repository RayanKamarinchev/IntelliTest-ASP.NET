using IntelliTest.Data.Entities;
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
            FirstTest = new Test()
            {
                AverageScore = 67.5m,
                Color1 = "#358DF1",
                Color2 = "#2383f0",
                Title = "Електромагнитни вълни",
                CreatedOn = new DateTime(2023, 2, 26, 19, 53, 6, 58, DateTimeKind.Local).AddTicks(7307),
                Description = "Просто тест",
                Grade = 10,
                IsDeleted = false,
                School = "ППМГ Добри Чинтулов",
                Subject = "Физика",
                Time = 30,
                Students = 15,
                MaxScore = 20,
                Id=1
            };
            builder.Entity<Test>()
                   .HasData(FirstTest);
            OpenQuestion = new OpenQuestion()
            {
                Answer = "Az",
                Id = 1,
                IsDeleted = false,
                Order = 0,
                Text = "Koi suzdade testut",
                TestId = 1
            };
            builder.Entity<OpenQuestion>()
                   .HasData(OpenQuestion);
            ClosedQuestion = new ClosedQuestion()
            {
                AnswerIndex = 1,
                Answers = "Ti&Az&dvamata&nikoi",
                Id = 2,
                IsDeleted = false,
                Order = 1,
                Text = "Koi suzdade testut",
                TestId = 1 
            };
            builder.Entity<ClosedQuestion>()
                   .HasData(ClosedQuestion);
            base.OnModelCreating(builder);
        }

        public Test FirstTest { get; set; }
        public OpenQuestion OpenQuestion { get; set; }
        public ClosedQuestion ClosedQuestion { get; set; }

        public DbSet<Test> Tests { get; set; }
        public DbSet<OpenQuestion> OpenQuestions { get; set; }
        public DbSet<ClosedQuestion> ClosedQuestions { get; set; }
        public DbSet<User> Users { get; set; }
    }
}