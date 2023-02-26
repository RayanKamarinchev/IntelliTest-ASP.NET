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
            base.OnModelCreating(builder);
        }

        public Test FirstTest { get; set; }

        public DbSet<Test> Tests { get; set; }
        public DbSet<OpenQuestion> OpenQuestions { get; set; }
        public DbSet<ClosedQuestion> ClosedQuestions { get; set; }
        public DbSet<User> Users { get; set; }
    }
}