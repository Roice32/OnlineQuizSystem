﻿using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions
{
    public static class StoreQuizResultBodyExtension
    {
        public static Result StoreQuizResultBody(this WebApplication application, QuizResultBody quizResultBody)
        {
            using var scope = application.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

            context.QuizResultBodies.Add(quizResultBody);
            context.SaveChanges();

            return Result.Success();
        }
    }
}