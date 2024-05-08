using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class FetchQuizResultBodyExtension
    {
        public static async Task<FetchQuizResultBodyResponse> FetchQuizResultBodyAsync(this WebApplication application, Guid quizId, Guid userId)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();
            return await FetchQuizResultBodyAsync(context, quizId, userId);
        }

        public static async Task<FetchQuizResultBodyResponse> FetchQuizResultBodyAsync(RSMApplicationDbContext dbContext, Guid quizId, Guid userId)
        {
            List<Guid> questionIds = await dbContext.QuizResultBodies
                .AsNoTracking()
                .Where(q => q.QuizId == quizId && q.UserId == userId)
                .Select(q => q.QuestionIds)
                .FirstOrDefaultAsync();

            // PLACEHOLDER
            // Until we get questions database.
            List<QuestionBase> questions = null; /*dbContext.Questions
                .AsNoTracking()
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();*/

            List<QuestionResultBase> questionResults = await dbContext.QuestionResults
                .AsNoTracking()
                .Where(q => questionIds.Contains(q.QuestionId) && q.UserId == userId)
                .ToListAsync();

            if(questions == null || questionResults == null)
            {
                return null;
            }

            return new FetchQuizResultBodyResponse
            {
                Questions = questions,
                QuestionResults = questionResults
            };
        }

        internal static FetchQuizResultBodyResponse FetchQuizResultBody(RSMApplicationDbContext dbContext, Guid quizId, Guid userId)
        {
            throw new NotImplementedException();
        }

    }
}