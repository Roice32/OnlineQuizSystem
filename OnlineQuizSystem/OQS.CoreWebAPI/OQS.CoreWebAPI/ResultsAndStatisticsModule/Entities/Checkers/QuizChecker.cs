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

            Result<List<QuestionResultBase>> questionsResults = await CheckAndStoreAllQuestionsAsync(toBeChecked, questions, dbContext);
            if (questionsResults.IsFailure)
            {
                return Result.Failure(questionsResults.Error);
            }
            return await BuildAndStoreQuizResultHeaderAsync(toBeChecked, questionsResults.Value, dbContext);
        }

        private static async Task<Result<List<QuestionResultBase>>> CheckAndStoreAllQuestionsAsync(QuizSubmission toBeChecked, List<QuestionBase> questionsFromDb, ApplicationDbContext dbContext)
        {
            List<Guid> questionIds = questionsFromDb.Select(q => q.Id).ToList();
            bool qaPairNotBelongingToQuiz = toBeChecked.QuestionAnswerPairs
                .Any(qaPair => !questionIds.Contains(qaPair.QuestionId));

            if (qaPairNotBelongingToQuiz)
            {
                return Result.Failure <List<QuestionResultBase>>(
                    new Error("QuizChecker.StrayAnswer",
                    "QuizSubmission contains answer to question not belonging to this quiz."));
            }

            List<QuestionResultBase> questionsResults = [];
            foreach (var question in questionsFromDb)
            {
                QuestionAnswerPairBase qaPair = toBeChecked.QuestionAnswerPairs
                    .FirstOrDefault(qaPair => qaPair.QuestionId == question.Id);
                QuestionResultBase questionResult = QuestionChecker.CheckQuestion(toBeChecked.TakenBy, qaPair, question);
                await StoreQuestionResultExtension.StoreQuestionResultAsync(dbContext, questionResult);
                questionsResults.Add(questionResult);
            }

            return questionsResults;
        }

        private static async Task<Result> BuildAndStoreQuizResultHeaderAsync(QuizSubmission toBeChecked,
            List<QuestionResultBase> questionsResults,
            ApplicationDbContext dbContext)
        {
            QuizResultHeader resultHeader = new(toBeChecked.QuizId,
                toBeChecked.TakenBy, toBeChecked.TimeElapsed);
            resultHeader.Score = 0;
            foreach (var questionResult in questionsResults)
            {
                resultHeader.Score += questionResult.Score;
                if (questionResult is ReviewNeededQuestionResult result &&
                    result.ReviewNeededResult == AnswerResult.Pending)
                    resultHeader.ReviewPending = true;
            }
            try
            {
                dbContext.QuizResultHeaders.Add(resultHeader);
                await dbContext.SaveChangesAsync();
            } catch (Exception e)
            {
                return Result.Failure(new Error("QuizChecker.QuizResultHeaderSaveError", e.Message));
            }
            return Result.Success();
        }
    }
}