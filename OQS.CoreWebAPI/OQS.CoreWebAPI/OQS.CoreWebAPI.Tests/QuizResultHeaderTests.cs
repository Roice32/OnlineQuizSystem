using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuizResultHeaders;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Tests
{
    public class QuizResultHeaderTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task When_QuizResultHeader_And_QuizName_And_UserName_Is_Not_Null_Should_Return_FetchQuizResultHeaderResponse()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");

            // Act
            var result = await FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, quizId, userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Result<FetchQuizResultHeaderResponse>>();

            var response = result.Value;
            response.QuizId.Should().Be(quizId);
            response.UserId.Should().Be(userId);
        }

        [Fact]
        public void When_QuizResultHeader_Is_Null_Should_Return_Empty_Response()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");

            // Act
            var result = FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, quizId, userId).Result;

            // Assert
            result.Should().BeEquivalentTo(Result.Failure(Error.NullValue));
        }

        [Fact]
        public void When_QuizName_Is_Null_Should_Return_Empty_Response()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000004");

            // Act
            var result = FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, quizId, userId).Result;

            // Assert
            result.Should().BeEquivalentTo(Result.Failure(Error.NullValue));
        }

        [Fact]
        public void When_UserName_Is_Null_Should_Return_Empty_Response()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000004");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");

            // Act
            var result = FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, quizId, userId).Result;

            // Assert
            result.Should().BeEquivalentTo(Result.Failure(Error.NullValue));
        }

        [Fact]
        public void When_QuizResultHeader_Is_Null_Should_Return_Failure()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");

            // Act
            var result = UpdateHeaderUponAnswerReviewExtension.UpdateHeaderUponAnswerReviewAsync(dbContext, quizId, userId).Result;
            result.Should().BeEquivalentTo(Result.Failure(Error.NullValue));
        }

        [Fact]
        public void When_QuestionIds_Is_Null_Should_Return_Failure()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000004");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");

            // Act
            var result = UpdateHeaderUponAnswerReviewExtension.UpdateHeaderUponAnswerReviewAsync(dbContext, quizId, userId).Result;
            result.Should().BeEquivalentTo(Result.Failure(Error.NullValue));
        }

      /*  [Fact]
        public void When_QaPair_Is_ReviewNeededQuestionResult_And_ReviewNeededResult_Is_Pending_Should_Return_Success()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");

            // Act
            var result = UpdateHeaderUponAnswerReviewExtension.UpdateHeaderUponAnswerReviewAsync(dbContext, quizId, userId).Result;
            result.Should().BeEquivalentTo(Result.Success());
            var updatedResult = FetchQuizResultBodyExtension.FetchQuizResultBodyAsync(dbContext, quizId, userId).Result.Value;
            updatedResult.Should().NotBeNull();
            updatedResult.ReviewPending.Should().BeTrue();
        }

        [Fact]
        public void When_QaPair_Is_Not_ReviewNeededQuestionResult_Should_Return_None()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");

            // Act
            var result = UpdateHeaderUponAnswerReviewExtension.UpdateHeaderUponAnswerReviewAsync(dbContext, quizId, userId).Result;
            result.Should().BeEquivalentTo(Result.Failure(Error.None));
        }
        */
    }
}