using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class GetQuizResultHeader
    {
        public static QuizResultHeader GetResultHeader(this WebApplication application, Guid QuizId, Guid UserId)
        {

            using var scope = application.Services.CreateScope();
            using (var dbContext = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>())
            {
                foreach (var line in dbContext.QuizResultHeaders)
                {
                    if (line.UserId == UserId && line.QuizId == QuizId)
                    {
                        QuizResultHeader quizFound = new QuizResultHeader(QuizId, UserId, line.CompletionTime);
                        quizFound.SubmittedAt = line.SubmittedAt;
                        quizFound.Score = line.Score;
                        quizFound.ReviewPending = line.ReviewPending;
                        return quizFound;
                    }
                }
            }
            return null;
        }
    }
}
