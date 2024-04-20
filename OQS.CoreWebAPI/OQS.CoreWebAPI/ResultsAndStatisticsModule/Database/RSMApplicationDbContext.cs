using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Database
{
    public class RSMApplicationDbContext : DbContext
    {
        public RSMApplicationDbContext(DbContextOptions<RSMApplicationDbContext> options) : 
            base(options) {}

        //public DbSet<QuizResultHeader> QuizResultHeaders { get; set; }
        //public DbSet<QuizResultBody> QuizResultBodies { get; set; }
        public DbSet<QuestionResultBase> QuestionResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionResultBase>().HasKey(qr => new { qr.UserId, qr.QuestionId });
        }
    }
}
