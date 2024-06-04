using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuizResultHeaders;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace OQS.CoreWebAPI.Tests.ResultsAndStatisticsTests
{
    public class QuizResultHeaderTests : ApplicationContextForTesting
    {
        // NEEDS MODIFYING!
        [Fact]
        public async Task Given_ValidIdsPair_When_FetchQuestionResultHeaderIsCalled_Then_CorrectHeaderResponseIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");

            // Act
            var result = (await FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, quizId)).Value;

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<FetchQuizResultHeaderResponse>();
            result.QuizName.Should().Be("Quiz1");
            result.UserName.Should().Be("FirstName1 LastName1");
            result.Score.Should().Be(13);
            result.ReviewPending.Should().BeFalse();
        }

        // NEEDS MODIFYING!
        [Fact]
        public async Task Given_IdsPairForNonexistentQuizResultHeader_When_FetchQuizResultHeaderIsCalled_Then_NullValueIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");

            // Act
            var result = await FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, quizId);
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.NullValue);
        }

        [Fact]
        public async Task When_StoreQuizResultHeaderIsCalled_Then_DatabaseEntryIsCreated()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");

            var header = new QuizResultHeader
            (
                resultId: Guid.NewGuid(),
                quizId: quizId,
                userId: userId
            );
            header.Score = 2;
            header.ReviewPending = false;

            // Act
            await StoreQuizResultHeaderExtension.StoreQuizResultHeaderAsync(dbContext, header);

            // Assert
            var storedHeader = await FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, quizId);
            storedHeader.IsSuccess.Should().BeTrue();
            storedHeader.Value.Score.Should().Be(2);
            storedHeader.Value.ReviewPending.Should().BeFalse();
        }

        [Fact]
        public void Given_IdsPairForNonexistentQuizResult_When_UpdateHeaderUponAnswerReviewIsCalled_Then_NullValueIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");

            // Act
            var result = UpdateHeaderUponAnswerReviewExtension.UpdateHeaderUponAnswerReviewAsync(dbContext, quizId).Result;
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.NullValue);
        }

        [Fact]
        public async Task When_UpdateHeaderUponAnswerReviewIsCalled_Then_QuizResultHeaderIsUpdated()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");

            // Act
            await UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, userId, questionId, 3);
            var result = await UpdateHeaderUponAnswerReviewExtension.UpdateHeaderUponAnswerReviewAsync(dbContext, userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var updatedHeader = await FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, quizId);
            updatedHeader.IsSuccess.Should().BeTrue();
            updatedHeader.Value.Score.Should().Be(3);
            updatedHeader.Value.ReviewPending.Should().BeFalse();
        }

        [Fact]
        public async Task Given_MoreThanOnePendingAnswer_When_UpdateHeaderUponAnswerReviewIsCalled_Then_QuizResultHeaderIsUpdatedWithReviewPending()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");
            // Does not exists, but leaves one pending answer in the result.
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000009");

            // Act
            await UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, userId, questionId, 3);
            var result = await UpdateHeaderUponAnswerReviewExtension.UpdateHeaderUponAnswerReviewAsync(dbContext, userId);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var updatedHeader = await FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, quizId);
            updatedHeader.IsSuccess.Should().BeTrue();
            updatedHeader.Value.Score.Should().Be(0);
            updatedHeader.Value.ReviewPending.Should().BeTrue();
        }
    }
}
