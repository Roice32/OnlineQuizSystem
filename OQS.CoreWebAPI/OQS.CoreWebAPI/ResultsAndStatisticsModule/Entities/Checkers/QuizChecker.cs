using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.Checkers
{
    public abstract class QuizChecker
    {
        public static void CheckQuiz(QuizSubmission toBeChecked, RSMApplicationDbContext dbContext)
        {
            Quiz quizFromDb = FetchQuizFromDB(toBeChecked.QuizId, dbContext);
            QuizResultBody resultBody = BuildQuizResultBody(toBeChecked, quizFromDb.Questions, dbContext);
            QuizResultHeader resultHeader = BuildQuizResultHeader(toBeChecked, resultBody, dbContext);
            StoreQuizResult(resultHeader, resultBody, dbContext);
        }
        private static Quiz FetchQuizFromDB(Guid QuizId, RSMApplicationDbContext dbContext)
        {
            Quiz quizFromDb = null; /*dbContext.Quizzes
                .AsNoTracking()
                .FirstOrDefault(q => q.Id == QuizId);*/
            return null;
        }
        private static QuizResultBody BuildQuizResultBody(QuizSubmission toBeChecked, List<QuestionBase> questionsFromDb, RSMApplicationDbContext dbContext)
        {
            QuizResultBody resultBody = new QuizResultBody(toBeChecked.QuizId,
                toBeChecked.TakenBy,
                toBeChecked.QuestionAnswerPairs.Select(qaPair => qaPair.QuestionId).ToList());
            foreach (var question in questionsFromDb)
            {
                QuestionAnswerPairBase qaPair = toBeChecked.QuestionAnswerPairs
                    .FirstOrDefault(qaPair => qaPair.QuestionId == question.Id);
                dbContext.QuestionResults.Add(QuestionChecker.CheckQuestion(toBeChecked.TakenBy, qaPair, question));
            }

            dbContext.SaveChanges();
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

        private static void StoreQuizResult(QuizResultHeader resultHeader, QuizResultBody resultBody, RSMApplicationDbContext dbContext)
        {
            dbContext.QuizResultBodies.Add(resultBody);
            dbContext.QuizResultHeaders.Add(resultHeader);
            dbContext.SaveChanges();
        }
    }
}