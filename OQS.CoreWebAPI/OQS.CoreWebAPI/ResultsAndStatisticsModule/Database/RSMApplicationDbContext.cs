using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Database
{
    public class RSMApplicationDbContext : DbContext
    {
        public RSMApplicationDbContext(DbContextOptions<RSMApplicationDbContext> options) : 
            base(options) {}

        public DbSet<QuizResultHeader> QuizResultHeaders { get; set; }

        public DbSet<QuizResultBody> QuizResultBodies { get; set; }

        public DbSet<QuestionResultBase> QuestionResults { get; set; }
        public DbSet<TrueFalseQuestionResult> TrueFalseQuestionResults { get; set; }
        public DbSet<ChoiceQuestionResult> ChoiceQuestionResults { get; set; }
        public DbSet<WrittenAnswerQuestionResult> WrittenAnswerQuestionResults { get; set; }
        public DbSet<ReviewNeededQuestionResult> ReviewNeededQuestionResults { get; set; }
        public object Users { get; internal set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuizResultHeader>().HasKey(qrh => new { qrh.UserId, qrh.QuizId });
            modelBuilder.Entity<QuizResultBody>().HasKey(qrb => new { qrb.UserId, qrb.QuizId});
            modelBuilder.Entity<QuestionResultBase>().HasKey(qr => new { qr.UserId, qr.QuestionId });
        }
    }
}
