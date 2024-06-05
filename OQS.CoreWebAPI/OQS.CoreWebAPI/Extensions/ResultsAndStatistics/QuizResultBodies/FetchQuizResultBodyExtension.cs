using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Contracts.CRUD;

namespace OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuizResultBodies
{
    public static class FetchQuizResultBodyExtension
    {
        public static async Task<Result<FetchQuizResultBodyResponse>> FetchQuizResultBodyAsync(this WebApplication application, Guid resultId)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await FetchQuizResultBodyAsync(context, resultId);
        }

        public static async Task<Result<FetchQuizResultBodyResponse>> FetchQuizResultBodyAsync(ApplicationDbContext dbContext, Guid resultId)
        {
            List<QuestionResultBase> questionResults = await dbContext.QuestionResults
                .AsNoTracking()
                .Where(q => q.ResultId == resultId)
                .ToListAsync();

            if (questionResults == null || questionResults.Count == 0)
            {
                Console.WriteLine("Error: QuestionResults not found in database");
                return Result.Failure<FetchQuizResultBodyResponse>(Error.NullValue);
            }

            List<QuestionBase> questions = dbContext.Questions
                .AsNoTracking()
                .Where(q => questionResults.Select(qr => qr.QuestionId).Contains(q.Id))
                .ToList();

            if (questions == null || questions.Count == 0)
            {
                Console.WriteLine("Error: Questions not found in database");
                return Result.Failure<FetchQuizResultBodyResponse>(Error.NullValue);
            }

            List<QuestionResponse> questionResponses = questions.Select(q => new QuestionResponse(q)).ToList();
            List<QuestionResultResponse> questionResultResponses = questionResults.Select(qr => new QuestionResultResponse(qr)).ToList();

            return new FetchQuizResultBodyResponse
            {
                Questions = questionResponses,
                QuestionResults = questionResultResponses
            };
        }
    }
}