using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuizResultHeaders
{
    public static class UpdateHeaderUponAnswerReviewExtension
    {
        public static Result UpdateHeaderUponAnswerReview(this WebApplication application, Guid userId, Guid quizId)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();
            UpdateHeaderUponAnswerReview(dbContext, userId, quizId);
            return Result.Success();
        }

        public static Result UpdateHeaderUponAnswerReview(RSMApplicationDbContext dbContext, Guid userId, Guid quizId)
        {
            var quizResultHeader = dbContext.QuizResultHeaders
                .FirstOrDefault(qrh => qrh.UserId == userId && qrh.QuizId == quizId);
            var questionIds = dbContext.QuizResultBodies
                .Where(qrb => qrb.UserId == userId && qrb.QuizId == quizId)
                .Select(qrb => qrb.QuestionIds)
                .FirstOrDefault();
            var questionResults = dbContext.QuestionResults
                .Where(qr => questionIds.Contains(qr.QuestionId) && qr.UserId == userId)
                .ToList();

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
            dbContext.SaveChanges();
            return Result.Success();
        }
    }
}