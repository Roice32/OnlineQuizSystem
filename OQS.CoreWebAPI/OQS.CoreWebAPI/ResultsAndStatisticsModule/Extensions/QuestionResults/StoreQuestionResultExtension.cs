using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.Shared;


namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults
{
    public static class StoreQuestionResultExtension
    {
        public static Result StoreResult(this WebApplication application, QuestionResultBase questionResult)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            context.QuestionResults.Add(questionResult);
            context.SaveChanges();
            return Result.Success();
        }
    }
}