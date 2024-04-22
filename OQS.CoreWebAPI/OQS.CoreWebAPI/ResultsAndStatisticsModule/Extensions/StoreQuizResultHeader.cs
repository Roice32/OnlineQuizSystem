using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class StoreQuizResultHeader
    {
        public static void StoreResultHeader(this WebApplication application, QuizResultHeader quizResultHeader)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();
            dbContext.QuizResultHeaders.Add(quizResultHeader);
            dbContext.SaveChanges();
        }
    }
}
