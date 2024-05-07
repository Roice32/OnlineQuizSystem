﻿using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this WebApplication application)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();
            context.Database.Migrate();
        }
    }
}
