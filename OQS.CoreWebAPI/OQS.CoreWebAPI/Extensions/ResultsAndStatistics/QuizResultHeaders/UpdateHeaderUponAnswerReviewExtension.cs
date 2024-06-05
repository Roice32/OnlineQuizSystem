using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuizResultHeaders
{
    public static class UpdateHeaderUponAnswerReviewExtension
    {
        public static async Task<Result> UpdateHeaderUponAnswerReviewAsync(this WebApplication application, Guid resultId)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await UpdateHeaderUponAnswerReviewAsync(dbContext, resultId);
        }

        public static async Task<Result> UpdateHeaderUponAnswerReviewAsync(ApplicationDbContext dbContext, Guid resultId)
        {
            var quizResultHeader = await dbContext.QuizResultHeaders
                .FirstOrDefaultAsync(qrh => qrh.ResultId == resultId);

            if (quizResultHeader is null)
            {
                Console.WriteLine("Error: QuizResultHeader not found in database");
                return Result.Failure(Error.NullValue);
            }

            var questionResults = await dbContext.QuestionResults
                .Where(qr => qr.ResultId == resultId)
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