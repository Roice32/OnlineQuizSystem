using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class UpdateQuestionResult
    {
        public static void UpdateResult(this WebApplication application, QuestionResultBase questionResult)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();
            context.QuestionResults.Update(questionResult);
            context.SaveChanges();
        }                                                                                                   
    }
}
