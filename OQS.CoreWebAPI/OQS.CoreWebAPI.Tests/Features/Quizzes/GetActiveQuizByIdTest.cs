using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Quizzes;

using Xunit;

namespace OQS.CoreWebAPI.Tests.Features.Quizzes
{
    public class GetActiveQuizByIdTest
    {
        [Fact]
        public async Task Handle_ValidActiveQuizId_ReturnsActiveQuizResponse()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            await using var context = new ApplicationDBContext(options);
            var activeQuizId = Guid.NewGuid();
            var quiz = new Quiz();
            var activeQuiz = new ActiveQuiz { Id = activeQuizId, Quiz = quiz};
            context.ActiveQuizzes.Add(activeQuiz);
            await context.SaveChangesAsync();
            
            var query = new GetActiveQuizById.Query(activeQuizId);
            var handler = new GetActiveQuizById.Handler(context);
            var result = await handler.Handle(query, CancellationToken.None);
            
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(activeQuizId, result.Value.Id);
        }

        [Fact]
        public async Task Handle_InvalidActiveQuizId_ReturnsError()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            await using var context = new ApplicationDBContext(options);
            
            var invalidActiveQuizId = Guid.NewGuid();
            var query = new GetActiveQuizById.Query(invalidActiveQuizId);
            var handler = new GetActiveQuizById.Handler(context);
            var result = await handler.Handle(query, CancellationToken.None);
            
            Assert.True(result.IsFailure);
            Assert.Equal(404, result.Error.Code);
            Assert.Equal("Active Quiz not found", result.Error.Message);
        }
    }
}
