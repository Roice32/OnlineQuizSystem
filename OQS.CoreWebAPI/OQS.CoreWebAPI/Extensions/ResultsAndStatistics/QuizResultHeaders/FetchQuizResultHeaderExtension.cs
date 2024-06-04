using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuizResultHeaders
{
    public static class FetchQuizResultHeaderExtension
    {
        public static async Task<Result<FetchQuizResultHeaderResponse>> FetchQuizResultHeaderAsync(this WebApplication application, Guid resultId)
        {
            using var scope = application.Services.CreateScope();
            using (var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                return await FetchQuizResultHeaderAsync(dbContext, resultId);
        }

        public static async Task<Result<FetchQuizResultHeaderResponse>> FetchQuizResultHeaderAsync(ApplicationDbContext dbContext, Guid resultId)
        {
            var quizResultHeader = await dbContext.QuizResultHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync(quiz => quiz.ResultId == resultId);

            if (quizResultHeader == null)
            {
                Console.WriteLine("Error: QuizResultHeader not found in database");
                return Result.Failure<FetchQuizResultHeaderResponse>(Error.NullValue);
            }

            var quizName = await dbContext
                .Quizzes
                .AsNoTracking()
                .Where(q => q.Id == quizResultHeader.QuizId)
                .Select(q => q.Name)
                .FirstOrDefaultAsync();

            var userName = await dbContext
                .Users
                .AsNoTracking()
                .Where(u => u.Id == quizResultHeader.UserId.ToString())
                .Select(u => u.FirstName + ' ' + u.LastName)
                .FirstOrDefaultAsync();


            return new FetchQuizResultHeaderResponse
            {
                ResultId = resultId,
                QuizId = quizResultHeader.QuizId,
                UserId = quizResultHeader.UserId,
                SubmittedAtUtc = quizResultHeader.SubmittedAtUtc,
                Score = quizResultHeader.Score,
                ReviewPending = quizResultHeader.ReviewPending,
                UserName = userName,
                QuizName = quizName
            };
        }
    }
}