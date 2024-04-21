﻿using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.Checkers
{
    public abstract class QuizChecker
    {
        public static void CheckQuiz(QuizSubmission toBeChecked)
        {
            Quiz quizFromDb = FetchQuizFromDB(toBeChecked.QuizId);
            QuizResultBody resultBody = BuildQuizResultBody(toBeChecked, quizFromDb.Questions);
            QuizResultHeader resultHeader = BuildQuizResultHeader(toBeChecked, resultBody);
            StoreQuizResult(resultHeader, resultBody);
        }
        private static Quiz FetchQuizFromDB(Guid QuizId)
        {
            // fetch quiz from DB
            // PLACEHOLDER
            return null;
        }
        private static QuizResultBody BuildQuizResultBody(QuizSubmission toBeChecked, List<QuestionBase> questionsFromDb)
        {
            QuizResultBody resultBody = new QuizResultBody(toBeChecked.QuizId,
                toBeChecked.TakenBy,
                toBeChecked.QuestionAnswerPairs.Select(qaPair => qaPair.QuestionId).ToList());
            foreach (var qaPair in toBeChecked.QuestionAnswerPairs)
            {
                QuestionBase questionFromDb = questionsFromDb.Find(q => q.Id == qaPair.QuestionId);
                // Insert in QuestionResults Table:
                    // (QuestionChecker.CheckQuestion(toBeChecked.TakenBy, qaPair, questionFromDb));
            }

            return resultBody;
        }
        private static QuizResultHeader BuildQuizResultHeader(QuizSubmission toBeChecked, QuizResultBody resultBody)
        {
            QuizResultHeader resultHeader = new QuizResultHeader(toBeChecked.QuizId,
                toBeChecked.TakenBy, toBeChecked.TimeElapsed);
            float totalScore = 0;
            foreach (var questionResult in resultBody.QuestionIds)
            {
                // PLACEHOLDER
                // Fetch result from DB
                QuestionResultBase questionResultBase = null;
                totalScore += questionResultBase.Score;
                if (questionResult is ReviewNeededQuestionResult &&
                    ((ReviewNeededQuestionResult)questionResultBase).ReviewNeededResult == AnswerResult.Pending)
                    resultHeader.ReviewPending = true;
            }
            resultHeader.Score = totalScore;
            return null;
        }

        private static void StoreQuizResult(QuizResultHeader resultHeader, QuizResultBody resultBody)
        {
            // store quiz result
        }

        ~QuizChecker()
        {
            // clean up
        }
    }
}