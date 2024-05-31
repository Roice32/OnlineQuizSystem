using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;

namespace OQS.CoreWebAPI.Database
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuizResultHeader>().HasKey(qrh => new { qrh.UserId, qrh.QuizId });
            modelBuilder.Entity<QuestionResultBase>().HasKey(qr => new { qr.UserId, qr.QuestionId });

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

        public DbSet<QuizResultHeader> QuizResultHeaders { get; set; }

        public DbSet<QuestionResultBase> QuestionResults { get; set; }
        public DbSet<TrueFalseQuestionResult> TrueFalseQuestionResults { get; set; }
        public DbSet<ChoiceQuestionResult> ChoiceQuestionResults { get; set; }
        public DbSet<WrittenAnswerQuestionResult> WrittenAnswerQuestionResults { get; set; }
        public DbSet<ReviewNeededQuestionResult> ReviewNeededQuestionResults { get; set; }
    }
}
