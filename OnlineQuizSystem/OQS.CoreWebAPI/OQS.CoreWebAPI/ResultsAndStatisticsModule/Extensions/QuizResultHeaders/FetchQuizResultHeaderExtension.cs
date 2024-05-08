using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class FetchQuizResultHeaderExtension
    {
        public static async Task<Result<FetchQuizResultHeaderResponse>> FetchQuizResultHeaderAsync(this WebApplication application, Guid QuizId, Guid UserId)
        {
            using var scope = application.Services.CreateScope();
            using (var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
            return await FetchQuizResultHeaderAsync(dbContext, QuizId, UserId);
        }

        public static async Task<Result<FetchQuizResultHeaderResponse>> FetchQuizResultHeaderAsync(ApplicationDbContext dbContext, Guid QuizId, Guid UserId)
        {
            var quizResultHeader = await dbContext.QuizResultHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync(quiz => quiz.QuizId == QuizId && quiz.UserId == UserId);

            // PLACEHOLDER
            // Until we get Quizzes table in database
            var quizName = "PLACEHOLDER"; /* await dbContext
                .Quizzes
                .AsNoTracking()
                .Select(q => q.Name)
                .FirstOrDefaultAsync(q => q.Id == QuizId); */

            var userName = "PLACEHOLDER"; /* await dbContext
                .Users
                .AsNoTracking()
                .Select(u => u.UserName)
                .FirstOrDefaultAsync(u => u.Id == UserId); */

            if (quizResultHeader == null || quizName == null || userName == null)
                return Result.Failure<FetchQuizResultHeaderResponse>(Error.NullValue);

            return new FetchQuizResultHeaderResponse
            {
                QuizId = QuizId,
                UserId = UserId,
                SubmittedAt = quizResultHeader.SubmittedAt,
                CompletionTime = quizResultHeader.CompletionTime,
                Score = quizResultHeader.Score,
                ReviewPending = quizResultHeader.ReviewPending,
                UserName = userName,
                QuizName = quizName
            };
        }
    }
}