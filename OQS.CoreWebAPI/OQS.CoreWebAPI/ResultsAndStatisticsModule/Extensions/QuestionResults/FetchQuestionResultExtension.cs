using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.Shared;


namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults
{
    public static class FetchQuestionResultExtension
    {
        public static Result<QuestionResultBase> FetchQuestionResult(this WebApplication application, Guid userId, Guid questionId)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();
            return FetchQuestionResult(dbContext, userId, questionId);
        }

        public static Result<QuestionResultBase> FetchQuestionResult(RSMApplicationDbContext dbContext, Guid userId, Guid questionId)
        {
            return dbContext.QuestionResults.FirstOrDefault(qr => qr.UserId == userId && qr.QuestionId == questionId);
        }
    }
}