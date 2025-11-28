using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Schedule> Schedules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "user1", Email = "user1@example.com", Password = "pass1" },
            new User { Id = 2, Username = "user2", Email = "user2@example.com", Password = "pass2" },
            new User { Id = 3, Username = "admin", Email = "admin@example.com", Password = "adminpass" }
        );

        modelBuilder.Entity<Document>().HasData(
            new Document { Id = 1, UserId = 1, Title = "Документ 1", Content = "Конфиденциальный документ пользователя 1" },
            new Document { Id = 2, UserId = 2, Title = "Документ 2", Content = "Конфиденциальный документ пользователя 2" },
            new Document { Id = 3, UserId = 1, Title = "Важный отчет", Content = "Финансовый отчет Q1" }
        );

        modelBuilder.Entity<Profile>().HasData(
            new Profile { Id = 1, UserId = 1, Bio = "Профиль пользователя 1", Settings = "{\"theme\":\"dark\"}" },
            new Profile { Id = 2, UserId = 2, Bio = "Профиль пользователя 2", Settings = "{\"theme\":\"light\"}" }
        );

        modelBuilder.Entity<Message>().HasData(
            new Message { Id = 1, SenderId = 1, ReceiverId = 2, Content = "Привет от пользователя 1", IsDraft = true },
            new Message { Id = 2, SenderId = 2, ReceiverId = 1, Content = "Привет от пользователя 2", IsDraft = false }
        );
    }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class Document
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}

public class Profile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Bio { get; set; }
    public string Settings { get; set; }
}

public class Message
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public string Content { get; set; }
    public bool IsDraft { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Schedule
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ExportType { get; set; }
    public DateTime ScheduleTime { get; set; }
    public string Status { get; set; } = "pending";
}
