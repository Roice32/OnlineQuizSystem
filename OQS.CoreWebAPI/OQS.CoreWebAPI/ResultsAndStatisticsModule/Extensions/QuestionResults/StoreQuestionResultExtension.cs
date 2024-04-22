using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;


namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults
{
    public static class StoreQuestionResultExtension
    {
        public static void StoreResult(this WebApplication application, QuestionResultBase questionResult)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();
            context.QuestionResults.Add(questionResult);
            context.SaveChanges();
        }
    }
}