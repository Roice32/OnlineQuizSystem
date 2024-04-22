using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class UpdateUponAnswerReview
    {
        public static void UpdateAnswerReview(this WebApplication application, QuizResultHeader quizResultHeader)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();
            dbContext.QuizResultHeaders.Update(quizResultHeader);
            dbContext.SaveChanges();                          
        }
    }
}
