using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;

namespace OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuestionResults
{
    public static class FetchQuestionResultExtension
    {
        public static async Task<QuestionResultBase> FetchQuestionResultAsync(this WebApplication application, Guid userId, Guid questionId)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await FetchQuestionResultAsync(dbContext, userId, questionId);
        }

        public static async Task<QuestionResultBase> FetchQuestionResultAsync(ApplicationDbContext dbContext, Guid userId, Guid questionId)
        {
            return await dbContext.QuestionResults.FirstOrDefaultAsync(qr => qr.UserId == userId && qr.QuestionId == questionId);
        }
    }
}