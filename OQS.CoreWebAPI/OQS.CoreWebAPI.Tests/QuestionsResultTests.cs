using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults;
using OQS.CoreWebAPI.Tests.SetUp;

namespace OQS.CoreWebAPI.Tests
{
    public class QuestionResultTests: ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_ValidIdsPair_When_FetchQuestionResultExtensionIsCalled_Then_CorrespondingResultIsReturnedAsync()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000005");

            // Act
            var result = await FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ReviewNeededQuestionResult>();
            
            var reviewNeededResult = result as ReviewNeededQuestionResult;
            reviewNeededResult.UserId.Should().Be(userId);
            reviewNeededResult.QuestionId.Should().Be(questionId);
            reviewNeededResult.ReviewNeededAnswer.Should().Be("SomePartiallyCorrectAnswer");
            reviewNeededResult.ReviewNeededResult.Should().Be(AnswerResult.PartiallyCorrect);
            reviewNeededResult.Score.Should().Be(3);
        }

        [Fact]
        public void Given_IdsPairForNonexistentResult_When_FetchQuestionResultExtensionIsCalled_Then_NullIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000006");

            // Act
            var result = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId).Result;

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void When_FetchQuestionResultExtensionIsCalled_Then_AllQuestionResultTypesCanBeReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId1 = Guid.Parse("00000000-0000-0000-0003-000000000001");
            var questionId2 = Guid.Parse("00000000-0000-0000-0003-000000000002");
            var questionId3 = Guid.Parse("00000000-0000-0000-0003-000000000003");
            var questionId4 = Guid.Parse("00000000-0000-0000-0003-000000000004");
            var questionId5 = Guid.Parse("00000000-0000-0000-0003-000000000005");

            // Act & Assert
            var result1 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId1).Result;
            result1.Should().BeOfType<TrueFalseQuestionResult>();
            var result2 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId2).Result;
            result2.Should().BeOfType<ChoiceQuestionResult>();
            var result3 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId3).Result;
            result3.Should().BeOfType<ChoiceQuestionResult>();
            var result4 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId4).Result;
            result4.Should().BeOfType<WrittenAnswerQuestionResult>();
            var result5 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId5).Result;
            result5.Should().BeOfType<ReviewNeededQuestionResult>();
        }
    }
}
