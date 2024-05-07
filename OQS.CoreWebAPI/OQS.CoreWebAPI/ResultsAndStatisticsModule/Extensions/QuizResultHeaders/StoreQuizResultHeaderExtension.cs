using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuizResultHeaders
{
    public static class StoreQuizResultHeaderExtension
    {
        public static Result StoreQuizResultHeader(this WebApplication application, QuizResultHeader quizResultHeader)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            dbContext.QuizResultHeaders.Add(quizResultHeader);
            dbContext.SaveChanges();
            return Result.Success();
        }
    }
}
