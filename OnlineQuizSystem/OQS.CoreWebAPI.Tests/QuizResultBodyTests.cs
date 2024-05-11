using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions;
using OQS.CoreWebAPI.Tests.SetUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Tests
{
    public class QuizResultBodyTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_ValidIdsPair_When_FetchQuestionResultBodyIsCalled_Then_CorrectBodyResponseIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");

            // Act
            var result = (await FetchQuizResultBodyExtension.FetchQuizResultBodyAsync(dbContext, quizId, userId)).Value;

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<FetchQuizResultBodyResponse>();

            List<Guid> questionIds = await dbContext.QuizResultBodies
                .AsNoTracking()
                .Where(q => q.UserId == userId && q.QuizId == quizId)
                .Select(q => q.QuestionIds)
                .FirstOrDefaultAsync();

            questionIds.Should().NotBeNull();   
            questionIds.Should().Contain(
                new List<Guid>
                {
                    Guid.Parse("00000000-0000-0000-0003-000000000007"),
                    Guid.Parse("00000000-0000-0000-0003-000000000008")

                  });
        }

        [Fact]
        public void Given_IdsPairForNonexistentQuizResultBody_When_FetchQuizResultBodyIsCalled_Then_NullValueIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");

            // Act
            var result = FetchQuizResultBodyExtension.FetchQuizResultBodyAsync(dbContext, quizId, userId).Result;
            result.IsFailure.Should().BeTrue();
        } 
    }
}
