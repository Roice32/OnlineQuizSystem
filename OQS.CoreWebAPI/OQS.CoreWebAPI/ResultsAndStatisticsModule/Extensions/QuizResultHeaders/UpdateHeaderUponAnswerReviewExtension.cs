using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuizResultHeaders
{
    public static class UpdateHeaderUponAnswerReviewExtension
    {
        public static async Task<Result> UpdateHeaderUponAnswerReviewAsync(this WebApplication application, Guid userId, Guid quizId)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await UpdateHeaderUponAnswerReviewAsync(dbContext, userId, quizId);
        }

        public static async Task<Result> UpdateHeaderUponAnswerReviewAsync(ApplicationDbContext dbContext, Guid userId, Guid quizId)
        {
            var quizResultHeader = await dbContext.QuizResultHeaders
                .FirstOrDefaultAsync(qrh => qrh.UserId == userId && qrh.QuizId == quizId);
            var questionIds = await dbContext.QuizResultBodies
                .Where(qrb => qrb.UserId == userId && qrb.QuizId == quizId)
                .Select(qrb => qrb.QuestionIds)
                .FirstOrDefaultAsync();
            var questionResults = await dbContext.QuestionResults
                .Where(qr => questionIds.Contains(qr.QuestionId) && qr.UserId == userId)
                .ToListAsync();

            // PLACEHOLDER
            // Only for testing API till we get quizzes database.
            if(quizResultHeader is null || questionIds is null || questionResults is null)
            {
                return Result.Failure(Error.NullValue);
            }

            quizResultHeader.Score = 0;
            quizResultHeader.ReviewPending = false;
            foreach (var qaPair in questionResults)
            {
                quizResultHeader.Score += qaPair.Score;
                if (qaPair is ReviewNeededQuestionResult &&
                                       ((ReviewNeededQuestionResult)qaPair).ReviewNeededResult == AnswerResult.Pending)
                    quizResultHeader.ReviewPending = true;
            }

            dbContext.QuizResultHeaders.Update(quizResultHeader);
            await dbContext.SaveChangesAsync();

            return Result.Success();
        }
    }
}