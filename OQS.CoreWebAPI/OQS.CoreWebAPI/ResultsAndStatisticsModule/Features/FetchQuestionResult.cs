using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;


namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class FetchQuestionResult
    {
        public static QuestionResultBase FetchResult(this WebApplication application, Guid userId, Guid questionId)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();
            return context.QuestionResults.FirstOrDefault(qr => qr.UserId == userId && qr.QuestionId == questionId);
        }
    }
}