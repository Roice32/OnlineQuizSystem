using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class MigrationExternsions
    {
        public static void ApplyMigrations(this WebApplication application)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
