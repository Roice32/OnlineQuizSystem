using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Database;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class FetchQuizResultBodyExtension
    {
        public static async Task<Result<FetchQuizResultBodyResponse>> FetchQuizResultBodyAsync(this WebApplication application, Guid quizId, Guid userId)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await FetchQuizResultBodyAsync(context, quizId, userId);
        }

        public static async Task<Result<FetchQuizResultBodyResponse>> FetchQuizResultBodyAsync(ApplicationDbContext dbContext, Guid quizId, Guid userId)
        {
            List<QuestionBase> questions = dbContext.Questions
                .AsNoTracking()
                .Where(q => q.QuizId == quizId)
                .ToList();

            if (questions == null)
            {
                return Result.Failure<FetchQuizResultBodyResponse>(Error.NullValue);
            }

            List<QuestionResultBase> questionResults = await dbContext.QuestionResults
                .AsNoTracking()
                .Where(q => questions.Select(q => q.Id).Contains(q.QuestionId) && q.UserId == userId)
                .ToListAsync();

            if (questions == null || questionResults == null)
            {
                return Result.Failure<FetchQuizResultBodyResponse>(Error.NullValue);
            }

            return new FetchQuizResultBodyResponse
            {
                Questions = questions,
                QuestionResults = questionResults
            };
        }
    }
}