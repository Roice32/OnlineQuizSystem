﻿using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults
{
    public static class UpdateQuestionResultExtension
    {
        public static void UpdateQuestionResult(this WebApplication application, Guid userId, Guid questionId, float score)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>();
            UpdateQuestionResult(dbContext, userId, questionId, score);
        }

        public static void UpdateQuestionResult(RSMApplicationDbContext dbContext, Guid userId, Guid questionId, float score)
        {
            var uncastedQuestionResult = FetchQuestionResultExtension.FetchQuestionResult(dbContext, userId, questionId);
            var questionResult = uncastedQuestionResult as ReviewNeededQuestionResult;

            // PLACEHOLDER
            // Only for testing API till we get quizzes database.
            if(questionResult is null)
            {
                return;
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
        }
    }
}