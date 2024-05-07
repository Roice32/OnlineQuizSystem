using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class FetchQuizResultHeaderExtension
    {
        public static Result<FetchQuizResultHeaderResponse> FetchQuizResultHeader(this WebApplication application, Guid QuizId, Guid UserId)
        {
            using var scope = application.Services.CreateScope();
            using (var dbContext = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>())
            return FetchQuizResultHeader(dbContext, QuizId, UserId);
        }

        public static Result<FetchQuizResultHeaderResponse> FetchQuizResultHeader(RSMApplicationDbContext dbContext, Guid QuizId, Guid UserId)
        {
            var quizResultHeader = dbContext.QuizResultHeaders
                .AsNoTracking()
                .FirstOrDefault(quiz => quiz.QuizId == QuizId && quiz.UserId == UserId);

            // PLACEHOLDER
            // Until we get Quizzes table in database
            var quizName = "PLACEHOLDER"; /* dbContext.Quizzes
                .AsNoTracking()
                .Select(q => q.Name)
                .FirstOrDefault(q => q.Id == QuizId); */

            var userName = "PLACEHOLDER"; /* dbContext.Users
                .AsNoTracking()
                .Select(u => u.UserName)
                .FirstOrDefault(u => u.Id == UserId); */

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