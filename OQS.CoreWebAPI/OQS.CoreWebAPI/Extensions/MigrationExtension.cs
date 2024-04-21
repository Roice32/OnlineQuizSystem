using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;

namespace OQS.CoreWebAPI.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication application) 
    {
        using var scope = application.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>(); 
        dbContext.Database.Migrate();
    }
}