using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        { }

        public DbSet<User> Users { get; set; }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<QuestionBase> Questions { get; set; }
        public DbSet<ChoiceQuestionBase> ChoiceQuestions { get; set; }
        public DbSet<TrueFalseQuestion> TrueFalseQuestions { get; set; }
        public DbSet<SingleChoiceQuestion> SingleChoiceQuestions { get; set; }
        public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
        public DbSet<WrittenAnswerQuestion> WrittenAnswerQuestions { get; set; }
        public DbSet<ReviewNeededQuestion> ReviewNeededQuestions { get; set; }

        public DbSet<QuizResultHeader> QuizResultHeaders { get; set; }

        public DbSet<QuestionResultBase> QuestionResults { get; set; }
        public DbSet<TrueFalseQuestionResult> TrueFalseQuestionResults { get; set; }
        public DbSet<ChoiceQuestionResult> ChoiceQuestionResults { get; set; }
        public DbSet<WrittenAnswerQuestionResult> WrittenAnswerQuestionResults { get; set; }
        public DbSet<ReviewNeededQuestionResult> ReviewNeededQuestionResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuizResultHeader>().HasKey(qrh => new { qrh.UserId, qrh.QuizId });
            modelBuilder.Entity<QuestionResultBase>().HasKey(qr => new { qr.UserId, qr.QuestionId });
        }
    }
}
