using Site_3.Models;
using Microsoft.EntityFrameworkCore;
using Site_3.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Site_3
{
    public class AdvancedDbContext : DbContext
    {
        public AdvancedDbContext(DbContextOptions<AdvancedDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ExportJob> ExportJobs { get; set; }
        public DbSet<SharedLink> SharedLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "advuser1", Email = "advuser1@example.com", Password = "adv1pass" },
                new User { Id = 2, Username = "advuser2", Email = "advuser2@example.com", Password = "adv2pass" }
            );

            modelBuilder.Entity<Message>().HasData(
                new Message { Id = 1, UserId = 1, Content = "Черновик 1", Status = "draft" },
                new Message { Id = 2, UserId = 2, Content = "Черновик 2", Status = "draft" }
            );

            modelBuilder.Entity<Schedule>().HasData(
                new Schedule { Id = 1, UserId = 1, LastRun = DateTime.Now.AddDays(-1) },
                new Schedule { Id = 2, UserId = 2, LastRun = DateTime.Now.AddDays(-1) }
            );

            modelBuilder.Entity<ExportJob>().HasData(
                new ExportJob { Id = 1, UserId = 1, Status = "completed", ExportData = "Данные экспорта user1" },
                new ExportJob { Id = 2, UserId = 2, Status = "completed", ExportData = "Данные экспорта user2" }
            );

            modelBuilder.Entity<SharedLink>().HasData(
                new SharedLink { Id = Guid.NewGuid().ToString(), UserId = 1, TargetId = "1", TargetType = "document" },
                new SharedLink { Id = Guid.NewGuid().ToString(), UserId = 2, TargetId = "2", TargetType = "document" }
            );
        }
    }
}
