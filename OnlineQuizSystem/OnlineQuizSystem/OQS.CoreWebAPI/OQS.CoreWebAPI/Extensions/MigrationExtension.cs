using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;

namespace OQS.CoreWebAPI.Extensions
{
    public static class MigrationExtension
    {
        public static void ApplyMigrations(this WebApplication application)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                context.Database.Migrate();
            }
        }
    }
}
