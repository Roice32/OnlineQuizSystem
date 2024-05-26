using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuizResultHeaders
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

            if (quizResultHeader is null)
            {
                return Result.Failure(Error.NullValue);
            }

            var questionIds = await dbContext.Questions
                .AsNoTracking()
                .Where(q => q.QuizId == quizId)
                .Select(q => q.Id)
                .ToListAsync();
            var questionResults = await dbContext.QuestionResults
                .Where(qr => questionIds.Contains(qr.QuestionId) && qr.UserId == userId)
                .ToListAsync();


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