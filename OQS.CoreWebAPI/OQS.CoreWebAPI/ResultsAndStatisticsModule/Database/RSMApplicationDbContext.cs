using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Database
{
    public class RSMApplicationDbContext : DbContext
    {
        public RSMApplicationDbContext(DbContextOptions<RSMApplicationDbContext> options) : 
            base(options) {}

            // Andra:
        //public DbSet<QuizResultHeader> QuizResultHeaders { get; set; }
        public DbSet<QuizResultBody> QuizResultBodies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionResultBase>().HasKey(qrb => new { qrb.UserId, qrb.QuestionId });

        }

        // Denisa:
        //public DbSet<QuestionResultBase> QuestionResults { get; set; }
        //public DbSet<TrueFalseQuestionResult> TrueFalseQuestionResults { get; set; }
        //public DbSet<ChoiceQuestionResult> ChoiceQuestionResults { get; set; }
        //public DbSet<WrittenAnswerQuestionResult> WrittenAnswerQuestionResults { get; set; }
        //public DbSet<ReviewNeededQuestionResult> ReviewNeededQuestionResults { get; set; }
    }
}
