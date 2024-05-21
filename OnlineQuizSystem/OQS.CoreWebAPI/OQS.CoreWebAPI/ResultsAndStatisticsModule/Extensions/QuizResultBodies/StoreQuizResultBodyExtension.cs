using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.Database;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuizResultBodies
{
    public static class StoreQuizResultBodyExtension
    {
        public static async Task StoreQuizResultBodyAsync(this WebApplication application, QuizResultBody quizResultBody)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await StoreQuizResultBodyAsync(dbContext, quizResultBody);
        }

        public static async Task StoreQuizResultBodyAsync(ApplicationDbContext dbContext, QuizResultBody quizResultBody)
        {
            await dbContext.QuizResultBodies.AddAsync(quizResultBody);
            await dbContext.SaveChangesAsync();
        }
    }
}