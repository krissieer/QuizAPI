using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Quiz.Models
{
    public class QuizDBContext : DbContext
    {
        public QuizDBContext()
        {
        }

        public QuizDBContext(DbContextOptions<QuizDBContext> options)
            : base(options)
        {
        }

        public DbSet<Attempt> Attempts { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Загружаем конфигурацию
                var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var connectionString = config.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                    throw new Exception("Connection string is missing! Add it to appsettings.json or environment variables.");

                optionsBuilder.UseNpgsql(connectionString);
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User 
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey("RoleId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Quizzes)
                .WithOne(r => r.Author)
                .HasForeignKey("AuthorId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Attemts)
                .WithOne(r => r.User)
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Restrict);

            // Question 
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Type)
                .WithMany()
                .HasForeignKey("QuestionTypeId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(r => r.Questions)
                .HasForeignKey("QuizId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Answers)
                .WithOne(r => r.Question)
                .HasForeignKey("QuestionId")
                .OnDelete(DeleteBehavior.Restrict);

            // Question.Options 
            modelBuilder.Entity<Question>()
                 .Property(q => q.Options)
                 .HasColumnType("jsonb");

            modelBuilder.Entity<Question>()
                .Property(q => q.CorrectAnswer)
                .HasColumnType("jsonb");


            // Quiz 
            modelBuilder.Entity<Quiz>()
                .HasOne(u => u.Author)
                .WithMany(r => r.Quizzes)
                .HasForeignKey("AuthorId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quiz>()
               .HasMany(q => q.Attempts)
               .WithOne(r => r.Quiz)
               .HasForeignKey("QuizId")
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quiz>()
               .HasMany(q => q.Questions)
               .WithOne(r => r.Quiz)
               .HasForeignKey("QuizId")
               .OnDelete(DeleteBehavior.Restrict);

            // Attempt 
            modelBuilder.Entity<Attempt>()
                .HasOne(u => u.User)
                .WithMany(r => r.Attemts)
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attempt>()
                .HasOne(u => u.Quiz)
                .WithMany(r => r.Attempts)
                .HasForeignKey("QuizId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attempt>()
                .HasMany(u => u.Answers)
                .WithOne(r => r.Attempt)
                .HasForeignKey("AttemptId")
                .OnDelete(DeleteBehavior.Restrict);

            // Answer 
            modelBuilder.Entity<Answer>()
                .HasOne(u => u.Attempt)
                .WithMany(r => r.Answers)
                .HasForeignKey("AttemptId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Answer>()
                .HasOne(u => u.Question)
                .WithMany(r => r.Answers)
                .HasForeignKey("QuestionId")
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
