using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (db.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                db.Database.Migrate();
            }
        }
    }
}
