﻿using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class FetchQuizResultHeaderExtension
    {
        public static async Task<FetchQuizResultHeaderResponse> FetchQuizResultHeaderAsync(this WebApplication application, Guid QuizId, Guid UserId)
        {
            using var scope = application.Services.CreateScope();
            using (var dbContext = scope.ServiceProvider.GetRequiredService<RSMApplicationDbContext>())
            return await FetchQuizResultHeaderAsync(dbContext, QuizId, UserId);
        }

        public static async Task<FetchQuizResultHeaderResponse> FetchQuizResultHeaderAsync(RSMApplicationDbContext dbContext, Guid QuizId, Guid UserId)
        {
            var quizResultHeader = await dbContext.QuizResultHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync(quiz => quiz.QuizId == QuizId && quiz.UserId == UserId);

            // PLACEHOLDER
            // Until we get Quizzes table in database
            var quizName = "PLACEHOLDER"; /* dbContext.Quizzes
                .AsNoTracking()
                .Select(q => q.Name)
                .FirstOrDefaultAsync(q => q.Id == QuizId); */

            var userName = "PLACEHOLDER"; /* dbContext.Users
                .AsNoTracking()
                .Select(u => u.UserName)
                .FirstOrDefaultAsync(u => u.Id == UserId); */

            if (quizResultHeader == null || quizName == null || userName == null)
                return null;

            return new FetchQuizResultHeaderResponse
            {
                QuizId = QuizId,
                UserId = UserId,
                SubmittedAt = quizResultHeader.SubmittedAt,
                CompletionTime = quizResultHeader.CompletionTime,
                Score = quizResultHeader.Score,
                ReviewPending = quizResultHeader.ReviewPending,
                UserName = userName,
                QuizName = quizName
            };
        }

        internal static FetchQuizResultHeaderResponse FetchQuizResultHeader(RSMApplicationDbContext dbContext, Guid quizId, Guid userId)
        {
            throw new NotImplementedException();
        }

    }
}