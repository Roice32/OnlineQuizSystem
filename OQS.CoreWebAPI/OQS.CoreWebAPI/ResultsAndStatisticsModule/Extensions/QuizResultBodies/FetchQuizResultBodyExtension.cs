using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Database;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class FetchQuizResultBodyExtension
    {
        public static Result<FetchQuizResultBodyResponse> FetchQuizResultBody(this WebApplication application, Guid quizId, Guid userId)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return FetchQuizResultBody(context, quizId, userId);
        }

        public static Result<FetchQuizResultBodyResponse> FetchQuizResultBody(ApplicationDbContext dbContext, Guid quizId, Guid userId)
        {
            List<Guid> questionIds = dbContext.QuizResultBodies
                .AsNoTracking()
                .Where(q => q.QuizId == quizId && q.UserId == userId)
                .Select(q => q.QuestionIds)
                .FirstOrDefault();

            // PLACEHOLDER
            // Until we get questions database.
            List<QuestionBase> questions = null; /*dbContext.Questions
                .AsNoTracking()
                .Where(q => questionIds.Contains(q.Id))
                .ToList();*/

            List<QuestionResultBase> questionResults = dbContext.QuestionResults
                .AsNoTracking()
                .Where(q => questionIds.Contains(q.QuestionId) && q.UserId == userId)
                .ToList();

            if(questions == null || questionResults == null)
            {
                return Result.Failure<FetchQuizResultBodyResponse>(Error.NullValue);
            }

            return new FetchQuizResultBodyResponse
            {
                Questions = questions,
                QuestionResults = questionResults
            };
        }
    }
}