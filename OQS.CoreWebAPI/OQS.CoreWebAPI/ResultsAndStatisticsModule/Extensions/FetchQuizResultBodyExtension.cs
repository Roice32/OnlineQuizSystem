using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class FetchQuizResultBodyExtension
    {
        public static GetQuizResultBodyResponse FetchQuizResultBody(this WebApplication application, QuizResultBody quizResultBody)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();

            var storedQuizResultBody = context.QuizResultBodies.Find(quizResultBody.UserId, quizResultBody.QuizId);

            List<QuestionBase> questionBases = new List<QuestionBase>();
            List<QuestionResultBase> questionResults = new List<QuestionResultBase>();

            foreach (var questionId in storedQuizResultBody.QuestionIds)
            {
                //PLACEHOLDER FOR DB

                var questionResult = context.QuestionResults.Find(storedQuizResultBody.UserId, questionId);
                questionResults.Add(questionResult);
            }

            return new GetQuizResultBodyResponse(questionBases, questionResults);
        }
    }
}
