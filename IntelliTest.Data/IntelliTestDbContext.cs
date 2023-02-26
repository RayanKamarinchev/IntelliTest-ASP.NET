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
        }

        public DbSet<Test> Tests { get; set; }
        public DbSet<OpenQuestion> OpenQuestions { get; set; }
        public DbSet<ClosedQuestion> ClosedQuestions { get; set; }
        public DbSet<User> Users { get; set; }
    }
}