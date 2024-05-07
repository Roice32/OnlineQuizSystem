using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults
{
    public static class UpdateQuestionResultExtension
    {
        public static Result UpdateQuestionResult(this WebApplication application, Guid userId, Guid questionId, float score)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            return UpdateQuestionResult(dbContext, userId, questionId, score);
        }

        public static Result UpdateQuestionResult(ApplicationDBContext dbContext, Guid userId, Guid questionId, float score)
        {
            var uncastedQuestionResult = FetchQuestionResultExtension.FetchQuestionResult(dbContext, userId, questionId);
            var questionResult = uncastedQuestionResult.Value as ReviewNeededQuestionResult;

            // PLACEHOLDER
            // Only for testing API till we get quizzes database.
            if(questionResult is null)
            {
                return Result.Failure(Error.NullValue);
            }

            questionResult.Score = score;
            if (score == 0)
            {
                questionResult.ReviewNeededResult = AnswerResult.Wrong;
            }
            else
            {
                // PLACEHOLDER
                int maxPossibleScore = 100; /* dbContext.Questions
                .AsNoTracking()
                .Select(q => q.AllocatedPoints)
                .FirstOrDefault(q => q.Id = questionId);*/
                if (score == maxPossibleScore)
                {
                    questionResult.ReviewNeededResult = AnswerResult.Correct;
                }
                else
                {
                    questionResult.ReviewNeededResult = AnswerResult.PartiallyCorrect;
                }
            }

            dbContext.QuestionResults.Update(questionResult);
            dbContext.SaveChanges();

            return Result.Success();
        }
    }
}