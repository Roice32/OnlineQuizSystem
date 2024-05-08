using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.Checkers
{
    public abstract class QuizChecker
    {
        public static async Task CheckQuizAsync(QuizSubmission toBeChecked, RSMApplicationDbContext dbContext)
        {
            Quiz quizFromDb = await FetchQuizFromDBAsync(toBeChecked.QuizId, dbContext);
            QuizResultBody resultBody = await BuildQuizResultBodyAsync(toBeChecked, quizFromDb.Questions, dbContext);
            QuizResultHeader resultHeader = BuildQuizResultHeader(toBeChecked, resultBody, dbContext);
            await StoreQuizResultAsync(resultHeader, resultBody, dbContext);
        }

        private static async Task<Quiz> FetchQuizFromDBAsync(Guid QuizId, RSMApplicationDbContext dbContext)
        {
            Quiz quizFromDb = null; /* await dbContext.Quizzes
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == QuizId);*/
            return null;
        }

        private static async Task<QuizResultBody> BuildQuizResultBodyAsync(QuizSubmission toBeChecked, List<QuestionBase> questionsFromDb, RSMApplicationDbContext dbContext)
        {
            QuizResultBody resultBody = new QuizResultBody(toBeChecked.QuizId,
                toBeChecked.TakenBy,
                toBeChecked.QuestionAnswerPairs.Select(qaPair => qaPair.QuestionId).ToList());
            foreach (var question in questionsFromDb)
            {
                QuestionAnswerPairBase qaPair = toBeChecked.QuestionAnswerPairs
                .FirstOrDefault(qaPair => qaPair.QuestionId == question.Id);
                await dbContext.QuestionResults.AddAsync(QuestionChecker.CheckQuestion(toBeChecked.TakenBy, qaPair, question));
            }
            await dbContext.SaveChangesAsync();
            return resultBody;
        }

        private static QuizResultHeader BuildQuizResultHeader(QuizSubmission toBeChecked,
            QuizResultBody resultBody,
            RSMApplicationDbContext dbContext)
        {
            QuizResultHeader resultHeader = new QuizResultHeader(toBeChecked.QuizId,
                toBeChecked.TakenBy, toBeChecked.TimeElapsed);
            resultHeader.Score = 0;
            foreach (var questionResultId in resultBody.QuestionIds)
            {
                QuestionResultBase questionResultBase = dbContext.QuestionResults
                    .AsNoTracking()
                    .FirstOrDefault(qr => qr.UserId == toBeChecked.TakenBy && qr.QuestionId == questionResultId);
                resultHeader.Score += questionResultBase.Score;
                if (questionResultBase is ReviewNeededQuestionResult &&
                    ((ReviewNeededQuestionResult)questionResultBase).ReviewNeededResult == AnswerResult.Pending)
                    resultHeader.ReviewPending = true;
            }
            return resultHeader;
        }

        private static async Task StoreQuizResultAsync(QuizResultHeader resultHeader, QuizResultBody resultBody, RSMApplicationDbContext dbContext)
        {
            await dbContext.QuizResultBodies.AddAsync(resultBody);
            await dbContext.QuizResultHeaders.AddAsync(resultHeader);
            await dbContext.SaveChangesAsync();
        }
    }
}