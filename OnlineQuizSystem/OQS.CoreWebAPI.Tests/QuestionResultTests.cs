using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions.QuestionResults;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;

namespace OQS.CoreWebAPI.Tests
{
    public class QuestionResultTests: ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_ValidIdsPair_When_FetchQuestionResultIsCalled_Then_CorrespondingResultIsReturnedAsync()
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
        }

        [Fact]
        public void Given_IdsPairForNonexistentResult_When_FetchQuestionResultIsCalled_Then_NullIsReturned()
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
        public void When_FetchQuestionResultIsCalled_Then_AllQuestionResultTypesCanBeReturned()
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
            result1.Should().NotBeNull();
            result1.Should().BeOfType<TrueFalseQuestionResult>();
            ((TrueFalseQuestionResult)result1).TrueFalseAnswerResult.Should().Be(AnswerResult.Correct);

            var result2 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId2).Result;
            result1.Should().NotBeNull();
            result2.Should().BeOfType<ChoiceQuestionResult>();
            var deserializedDictionary1 =
                JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(
                    ((ChoiceQuestionResult)result2).PseudoDictionaryChoicesResults);
            deserializedDictionary1.Should()
                .BeEquivalentTo(new Dictionary<string, AnswerResult>
                {
                    {"Option1", AnswerResult.Other},
                    {"Option2", AnswerResult.Correct},
                    {"Option3", AnswerResult.Other}
                });
            
            var result3 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId3).Result;
            result1.Should().NotBeNull();
            result3.Should().BeOfType<ChoiceQuestionResult>();
            var deserializedDictionary2 = 
                JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(
                    ((ChoiceQuestionResult)result3).PseudoDictionaryChoicesResults);
            deserializedDictionary2.Should()
                .BeEquivalentTo(new Dictionary<string, AnswerResult>
                {
                    { "Option1", AnswerResult.Wrong },
                    { "Option2", AnswerResult.Correct },
                    { "Option3", AnswerResult.Other },
                    { "Option4", AnswerResult.CorrectNotPicked }
                });

            var result4 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId4).Result;
            result4.Should().NotBeNull();
            result4.Should().BeOfType<WrittenAnswerQuestionResult>();
            ((WrittenAnswerQuestionResult)result4).WrittenAnswerResult.Should().Be(AnswerResult.Correct);
            
            var result5 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId5).Result;
            result5.Should().NotBeNull();
            result5.Should().BeOfType<ReviewNeededQuestionResult>();
            ((ReviewNeededQuestionResult)result5).ReviewNeededResult.Should().Be(AnswerResult.PartiallyCorrect);
        }

        [Fact]
        public async Task When_StoreQuestionResultIsCalled_Then_DatabaseEntryIsCreated()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000006");
            var questionResult = new TrueFalseQuestionResult
            (
                userId: userId,
                questionId: questionId,
                score: 0,
                trueFalseAnswerResult: AnswerResult.Wrong
            );

            // Act
            await StoreQuestionResultExtension.StoreQuestionResultAsync(dbContext, questionResult);
            var result = await FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TrueFalseQuestionResult>();
            ((TrueFalseQuestionResult)result).TrueFalseAnswerResult.Should().Be(AnswerResult.Wrong);
            result.Score.Should().Be(0);

        }

        [Fact]
        public void Given_IdsPairForNonexistentResult_When_UpdateQuestionResultIsCalled_Then_NullValueErrorIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var questionId1 = Guid.NewGuid();
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");

            // Act
            var result = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, userId, questionId1, 0).Result;
            result.Should().BeEquivalentTo(Result.Failure(Error.NullValue));
        }

        [Fact]
        public void Given_IdsPairForNonReviewNeededResult_When_UpdateQuestionResultIsCalled_Then_InvalidTypeErrorIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var questionId1 = Guid.Parse("00000000-0000-0000-0003-000000000001");
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");

            // Act
            var result = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, userId, questionId1, 0).Result;
            result.Should().BeEquivalentTo(Result.Failure(Error.InvalidType));
        }

        [Fact]
        public void Given_InvalidScore_When_UpdateQuestionResultIsCalled_Then_OutOfBoundsValueErrorIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000005");

            // Act & Assert
            var result1 = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, userId, questionId, 6.01f).Result;
            result1.Should().BeEquivalentTo(Result.Failure(Error.OutOfBoundsValue));

            var result2 = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, userId, questionId, -1).Result;
            result2.Should().BeEquivalentTo(Result.Failure(Error.OutOfBoundsValue));
        }

        [Fact]
        public void Given_IdsPairForResultAlreadyReviewed_When_UpdateQuestionResultIsCalled_Then_ConditionNotMetIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000005");

            // Act
            var result = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, userId, questionId, 2).Result;

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.ConditionNotMet);
        }

        [Fact]
        public void Given_ZeroScore_When_UpdateQuestionResultIsCalled_Then_WrongAnswerIsStoredInDatabase()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");

            // Act & Assert
            var result = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, userId, questionId, 0).Result;
            result.IsSuccess.Should().BeTrue();
            var updatedResult = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId).Result;
            updatedResult.Should().NotBeNull();
            updatedResult.Should().BeOfType<ReviewNeededQuestionResult>();
            ((ReviewNeededQuestionResult)updatedResult).ReviewNeededResult.Should().Be(AnswerResult.Wrong);
        }

        [Fact]
        public void Given_PartialScore_When_UpdateQuestionResultIsCalled_Then_PartiallyCorrectAnswerIsStoredInDatabase()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");

            // Act & Assert
            var result = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, userId, questionId, 1).Result;
            result.IsSuccess.Should().BeTrue();
            var updatedResult = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId).Result;
            updatedResult.Should().NotBeNull();
            updatedResult.Should().BeOfType<ReviewNeededQuestionResult>();
            ((ReviewNeededQuestionResult)updatedResult).ReviewNeededResult.Should().Be(AnswerResult.PartiallyCorrect);
        }

        [Fact]
        public void Given_MaxScore_When_UpdateQuestionResultIsCalled_Then_CorrectAnswerIsStoredInDatabase()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");

            // Act & Assert
            var result = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, userId, questionId, 5).Result;
            result.IsSuccess.Should().BeTrue();
            var updatedResult = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, userId, questionId).Result;
            updatedResult.Should().NotBeNull();
            updatedResult.Should().BeOfType<ReviewNeededQuestionResult>();
            ((ReviewNeededQuestionResult)updatedResult).ReviewNeededResult.Should().Be(AnswerResult.Correct);
        }
    }
}
