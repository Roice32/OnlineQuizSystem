using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults
{
    public static class StoreQuestionResultExtension
    {
        public static async Task StoreQuestionResultAsync(this WebApplication application, QuestionResultBase questionResult)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await StoreQuestionResultAsync(context, questionResult);
        }

        public static async Task StoreQuestionResultAsync(ApplicationDbContext dbContext, QuestionResultBase questionResult)
        {
            int count = await dbContext.QuestionResults.CountAsync();
            await dbContext.QuestionResults.AddAsync(questionResult);
            await dbContext.SaveChangesAsync();
            int newCount = await dbContext.QuestionResults.CountAsync();
        }
    }
}