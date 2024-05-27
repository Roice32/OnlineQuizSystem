using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;

namespace OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuizResultHeaders
{
    public static class StoreQuizResultHeaderExtension
    {
        public static async Task StoreQuizResultHeaderAsync(this WebApplication application, QuizResultHeader quizResultHeader)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await StoreQuizResultHeaderAsync(dbContext, quizResultHeader);
        }

        public static async Task StoreQuizResultHeaderAsync(ApplicationDbContext dbContext, QuizResultHeader quizResultHeader)
        {
            await dbContext.QuizResultHeaders.AddAsync(quizResultHeader);
            await dbContext.SaveChangesAsync();
        }
    }
}
