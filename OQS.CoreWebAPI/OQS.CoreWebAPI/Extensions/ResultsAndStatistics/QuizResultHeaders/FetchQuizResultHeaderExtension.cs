using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuizResultHeaders
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

            if (quizResultHeader == null)
            {
                return Result.Failure<FetchQuizResultHeaderResponse>(Error.NullValue);
            }

            var quizName = await dbContext
                .Quizzes
                .AsNoTracking()
                .Where(q => q.Id == QuizId)
                .Select(q => q.Name)
                .FirstOrDefaultAsync();

            var userName = await dbContext
                .Users
                .AsNoTracking()
                .Where(u => u.Id == UserId)
                .Select(u => u.Name)
                .FirstOrDefaultAsync();


            return new FetchQuizResultHeaderResponse
            {
                QuizId = QuizId,
                UserId = UserId,
                SubmittedAtUtc = quizResultHeader.SubmittedAtUtc,
                CompletionTime = quizResultHeader.CompletionTime,
                Score = quizResultHeader.Score,
                ReviewPending = quizResultHeader.ReviewPending,
                UserName = userName,
                QuizName = quizName
            };
        }
    }
}