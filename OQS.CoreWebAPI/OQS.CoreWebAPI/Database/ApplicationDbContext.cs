using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Entities.ActiveQuiz;

namespace OQS.CoreWebAPI.Database
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            
        }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserConnection>(entity=>
            {
                entity.HasKey(e => e.ConnectionId);
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Connections)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
           
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Connections)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<LiveQuizz>(entity=>
            {
                entity.HasKey(e => e.Code);
            });
        }

        public DbSet<QuestionBase> Questions { get;set; }
        public DbSet<ChoiceQuestionBase> ChoiceQuestions { get; set; }
        public DbSet<TrueFalseQuestion> TrueFalseQuestions{get;set;}
        public DbSet<SingleChoiceQuestion> SingleChoiceQuestions{get;set;}
        public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions{get;set;}
        public DbSet<WrittenAnswerQuestion> WrittenAnswerQuestions { get; set; }
        public DbSet<ReviewNeededQuestion> ReviewNeededQuestions { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<ActiveQuiz> ActiveQuizzes { get; set; }
    
        public DbSet<LiveQuizz> LiveQuizzes { get; set; }
    
        public DbSet<UserConnection> UserConnections { get; set; }

        public DbSet<Tag> Tags { get; set; }
    }
}