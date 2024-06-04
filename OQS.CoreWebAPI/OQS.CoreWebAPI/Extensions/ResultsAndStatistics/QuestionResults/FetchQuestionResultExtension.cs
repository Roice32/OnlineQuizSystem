using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;

namespace OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuestionResults
{
    public static class FetchQuestionResultExtension
    {
        public static async Task<QuestionResultBase> FetchQuestionResultAsync(this WebApplication application, Guid resultId, Guid questionId)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await FetchQuestionResultAsync(dbContext, resultId, questionId);
        }

        public static async Task<QuestionResultBase> FetchQuestionResultAsync(ApplicationDbContext dbContext, Guid resultId, Guid questionId)
        {
            return await dbContext.QuestionResults.FirstOrDefaultAsync(qr => qr.ResultId == resultId && qr.QuestionId == questionId);
        }
    }
}