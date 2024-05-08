using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults
{
    public static class StoreQuestionResultExtension
    {
        public static async Task StoreResult(this WebApplication application, QuestionResultBase questionResult)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.QuestionResults.AddAsync(questionResult);
        }
    }
}