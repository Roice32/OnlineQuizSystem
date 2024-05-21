using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.Checkers
{
    public abstract class QuizChecker
    {
        public static async Task<Result> CheckQuizAsync(QuizSubmission toBeChecked, ApplicationDbContext dbContext)
        {
            bool quizAlreadyTaken = await dbContext.QuizResultHeaders
                .AnyAsync(qrh => qrh.QuizId == toBeChecked.QuizId && qrh.UserId == toBeChecked.TakenBy);
            if (quizAlreadyTaken)
            {
                return Result.Failure(Error.DuplicateEntity);
            }

            Quiz quizFromDb = await dbContext.Quizzes
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == toBeChecked.QuizId);
            if (quizFromDb is null)
            {
                return Result.Failure(Error.NullValue);
            }

            var questions = await dbContext.Questions
                .AsNoTracking()
                .Where(q => q.QuizId == toBeChecked.QuizId)
                .ToListAsync();

            Result<QuizResultBody> resultBody = await BuildQuizResultBodyAsync(toBeChecked, questions, dbContext);
            if (resultBody.IsFailure)
            {
                return Result.Failure(resultBody.Error);
            }
            QuizResultHeader resultHeader = await BuildQuizResultHeaderAsync(toBeChecked, resultBody.Value, dbContext);
            return await StoreQuizResultAsync(resultHeader, resultBody.Value, dbContext);
        }

        private static async Task<Result<QuizResultBody>> BuildQuizResultBodyAsync(QuizSubmission toBeChecked, List<QuestionBase> questionsFromDb, ApplicationDbContext dbContext)
        {
            List<Guid> questionIds = questionsFromDb.Select(q => q.Id).ToList();
            bool qaPairNotBelongingToQuiz = toBeChecked.QuestionAnswerPairs
                .Any(qaPair => !questionIds.Contains(qaPair.QuestionId));

            if (qaPairNotBelongingToQuiz)
            {
                return Result.Failure<QuizResultBody>(
                    new Error("QuizChecker.StrayAnswer",
                    "QuizSubmission contains answer to question not belonging to this quiz."));
            }

            QuizResultBody resultBody = new QuizResultBody(toBeChecked.QuizId,
                toBeChecked.TakenBy,
                questionIds);
            foreach (var question in questionsFromDb)
            {
                QuestionAnswerPairBase qaPair = toBeChecked.QuestionAnswerPairs
                    .FirstOrDefault(qaPair => qaPair.QuestionId == question.Id);
                QuestionResultBase questionResult = QuestionChecker.CheckQuestion(toBeChecked.TakenBy, qaPair, question);
                await StoreQuestionResultExtension.StoreQuestionResultAsync(dbContext, questionResult);
            }

            return resultBody;
        }

        private static async Task<QuizResultHeader> BuildQuizResultHeaderAsync(QuizSubmission toBeChecked,
            QuizResultBody resultBody,
            ApplicationDbContext dbContext)
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

        private static async Task<Result> StoreQuizResultAsync(QuizResultHeader resultHeader, QuizResultBody resultBody, ApplicationDbContext dbContext)
        {
            await dbContext.QuizResultBodies.AddAsync(resultBody);
            await dbContext.QuizResultHeaders.AddAsync(resultHeader);
            await dbContext.SaveChangesAsync();
            return Result.Success();
        }
    }
}