// ApiDbContext.cs
using Site_2.Models;
using Microsoft.EntityFrameworkCore;
using Site_2.Models;

namespace Site_2
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        // DbSet'ы для каждой модели
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<FileRecord> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Заполнение начальными тестовыми данными
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "apiuser1", Email = "apiuser1@example.com", Password = "api1pass" },
                new User { Id = 2, Username = "apiuser2", Email = "apiuser2@example.com", Password = "api2pass" }
            );

            modelBuilder.Entity<Profile>().HasData(
                new Profile { Id = 1, UserId = 1, FirstName = "API User 1", LastName = "Smith", Email = "apiuser1@example.com" },
                new Profile { Id = 2, UserId = 2, FirstName = "API User 2", LastName = "Johnson", Email = "apiuser2@example.com" }
            );

            modelBuilder.Entity<FileRecord>().HasData(
                new FileRecord { Id = "file1", UserId = 1, FileName = "report.pdf", Content = "PDF Content 1" },
                new FileRecord { Id = "file2", UserId = 2, FileName = "invoice.pdf", Content = "PDF Content 2" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
