using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.ResultTypes;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Database
{
    public class RSMApplicationDbContext : DbContext
    {
        public RSMApplicationDbContext(DbContextOptions<RSMApplicationDbContext> options) : 
            base(options) {}

        //public DbSet<QuizResultHeader> QuizResultHeaders { get; set; }
        //public DbSet<QuizResultBody> QuizResultBodies { get; set; }
        public DbSet<QuestionResult> QuestionResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QuestionResult>().HasKey(qr => new { qr.UserId, qr.QuestionId });
        }
    }
}
