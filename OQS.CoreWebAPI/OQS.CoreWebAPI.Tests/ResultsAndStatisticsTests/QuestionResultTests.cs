using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace OQS.CoreWebAPI.Tests.ResultsAndStatisticsTests
{
    public class QuestionResultTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_ValidIdsPair_When_FetchQuestionResultIsCalled_Then_CorrespondingResultIsReturnedAsync()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000005");
            var resultId = Guid.NewGuid();

            var questionResult = new ReviewNeededQuestionResult(
                resultId: resultId,
                userId: userId,
                questionId: questionId,
                score: 5,
                reviewNeededAnswer: "Example answer",
                reviewNeededResult: AnswerResult.PartiallyCorrect
            );

            dbContext.QuestionResults.Add(questionResult);
            await dbContext.SaveChangesAsync();

            var seededResult = await dbContext.QuestionResults
                .FindAsync(new object[] { resultId, questionId });
            seededResult.Should().NotBeNull("seeding should create a record in the database");

            // Act
            var result = await FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, resultId, questionId);

            // Assert
            result.Should().NotBeNull("the result should not be null");
            result.Should().BeOfType<ReviewNeededQuestionResult>("the result should be of type ReviewNeededQuestionResult");

            var reviewNeededResult = result as ReviewNeededQuestionResult;
            reviewNeededResult.UserId.Should().Be(userId, $"expected userId to be {userId}");
            reviewNeededResult.QuestionId.Should().Be(questionId, $"expected questionId to be {questionId}");


            dbContext.QuestionResults.RemoveRange(dbContext.QuestionResults);
            dbContext.SaveChanges();
        }


        [Fact]
        public void Given_IdsPairForNonexistentResult_When_FetchQuestionResultIsCalled_Then_NullIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.NewGuid();

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

            var trueFalseQuestionResult = new TrueFalseQuestionResult(
                resultId: Guid.NewGuid(),
                userId: userId,
                questionId: questionId1,
                score: 1,
                trueFalseAnswerResult: AnswerResult.Correct
            );

            var choiceQuestionResult1 = new ChoiceQuestionResult(
                resultId: Guid.NewGuid(),
                userId: userId,
                questionId: questionId2,
                score: 1,
                pseudoDictionaryChoicesResults: JsonConvert.SerializeObject(new Dictionary<string, AnswerResult>
                {
                    { "Option1", AnswerResult.Other },
                    { "Option2", AnswerResult.Correct },
                    { "Option3", AnswerResult.Other }
                })
            );

            var choiceQuestionResult2 = new ChoiceQuestionResult(
                resultId: Guid.NewGuid(),
                userId: userId,
                questionId: questionId3,
                score: 1,
                pseudoDictionaryChoicesResults: JsonConvert.SerializeObject(new Dictionary<string, AnswerResult>
                {
                    { "Option1", AnswerResult.Wrong },
                    { "Option2", AnswerResult.Correct },
                    { "Option3", AnswerResult.Other },
                    { "Option4", AnswerResult.CorrectNotPicked }
                })
            );


            var writtenAnswerQuestionResult = new WrittenAnswerQuestionResult(
                resultId: Guid.NewGuid(),
                userId: userId,
                questionId: questionId4,
                score: 1,
                writtenAnswer: "Test Answer",
                writtenAnswerResult: AnswerResult.Correct
            );

            var reviewNeededQuestionResult = new ReviewNeededQuestionResult(
                resultId: Guid.NewGuid(),
                userId: userId,
                questionId: questionId5,
                score: 1,
                reviewNeededAnswer: "Some answer",
                reviewNeededResult: AnswerResult.PartiallyCorrect
            );

            dbContext.QuestionResults.AddRange(
                trueFalseQuestionResult,
                choiceQuestionResult1,
                choiceQuestionResult2,
                writtenAnswerQuestionResult,
                reviewNeededQuestionResult
            );
            dbContext.SaveChanges();

            // Act & Assert
            var result1 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, trueFalseQuestionResult.ResultId, questionId1).Result;
            result1.Should().NotBeNull();
            result1.Should().BeOfType<TrueFalseQuestionResult>();
            ((TrueFalseQuestionResult)result1).TrueFalseAnswerResult.Should().Be(AnswerResult.Correct);

            var result2 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, choiceQuestionResult1.ResultId, questionId2).Result;
            result2.Should().NotBeNull();
            result2.Should().BeOfType<ChoiceQuestionResult>();
            var deserializedDictionary1 =
                JsonConvert.DeserializeObject<Dictionary<string, AnswerResult>>(
                    ((ChoiceQuestionResult)result2).PseudoDictionaryChoicesResults);
            deserializedDictionary1.Should()
                .BeEquivalentTo(new Dictionary<string, AnswerResult>
                {
                    { "Option1", AnswerResult.Other },
                    { "Option2", AnswerResult.Correct },
                    { "Option3", AnswerResult.Other }
                });

            var result3 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, choiceQuestionResult2.ResultId, questionId3).Result;
            result3.Should().NotBeNull();
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

            var result4 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, writtenAnswerQuestionResult.ResultId, questionId4).Result;
            result4.Should().NotBeNull();
            result4.Should().BeOfType<WrittenAnswerQuestionResult>();
            ((WrittenAnswerQuestionResult)result4).WrittenAnswerResult.Should().Be(AnswerResult.Correct);

            var result5 = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, reviewNeededQuestionResult.ResultId, questionId5).Result;
            result5.Should().NotBeNull();
            result5.Should().BeOfType<ReviewNeededQuestionResult>();
            ((ReviewNeededQuestionResult)result5).ReviewNeededResult.Should().Be(AnswerResult.PartiallyCorrect);

            dbContext.QuestionResults.RemoveRange(dbContext.QuestionResults);
            dbContext.SaveChanges();
        }


        [Fact]
        public async Task When_StoreQuestionResultIsCalled_Then_DatabaseEntryIsCreated()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000006");
            var resultId = Guid.NewGuid();
            var questionResult = new TrueFalseQuestionResult
            (
                resultId: resultId,
                userId: userId,
                questionId: questionId,
                score: 0,
                trueFalseAnswerResult: AnswerResult.Wrong
            );

            // Act
            await StoreQuestionResultExtension.StoreQuestionResultAsync(dbContext, questionResult);
            var result = await FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, resultId, questionId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<TrueFalseQuestionResult>();
            ((TrueFalseQuestionResult)result).TrueFalseAnswerResult.Should().Be(AnswerResult.Wrong);
            result.Score.Should().Be(0);

            dbContext.QuestionResults.RemoveRange(dbContext.QuestionResults);
            dbContext.SaveChanges();
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
            var resultId = Guid.NewGuid();

            var trueFalseQuestionResult = new TrueFalseQuestionResult(
                resultId: resultId,
                userId: userId,
                questionId: questionId1,
                score: 5,
                trueFalseAnswerResult: AnswerResult.Correct
            );

            dbContext.QuestionResults.Add(trueFalseQuestionResult);
            dbContext.SaveChanges();

            // Act
            var result = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, resultId, questionId1, 0).Result;

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Code.Should().Be("Error.InvalidType");
            result.Error.Message.Should().Be("Invalid type.");

            dbContext.QuestionResults.RemoveRange(dbContext.QuestionResults);
            dbContext.SaveChanges();
        }


        [Fact]
        public void Given_InvalidScore_When_UpdateQuestionResultIsCalled_Then_OutOfBoundsValueErrorIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000005");
            var resultId = Guid.NewGuid();

            var reviewNeededQuestionResult = new ReviewNeededQuestionResult(
                resultId: resultId,
                userId: userId,
                questionId: questionId,
                score: 5,
                reviewNeededAnswer: "Some answer",
                reviewNeededResult: AnswerResult.Pending
            );

            dbContext.QuestionResults.Add(reviewNeededQuestionResult);
            dbContext.SaveChanges();

            // Act & Assert
            var result1 = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, resultId, questionId, 6.01f).Result;
            result1.IsFailure.Should().BeTrue();
            result1.Error.Code.Should().Be("Error.OutOfBoundsValue");
            result1.Error.Message.Should().Be("Value is out of bounds.");

            var result2 = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, resultId, questionId, -1).Result;
            result2.IsFailure.Should().BeTrue();
            result2.Error.Code.Should().Be("Error.OutOfBoundsValue");
            result2.Error.Message.Should().Be("Value is out of bounds.");

            dbContext.QuestionResults.RemoveRange(dbContext.QuestionResults);
            dbContext.SaveChanges();
        }

        [Fact]
        public void Given_IdsPairForResultAlreadyReviewed_When_UpdateQuestionResultIsCalled_Then_ConditionNotMetIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000005");
            var resultId = Guid.NewGuid();

            var reviewNeededQuestionResult = new ReviewNeededQuestionResult(
                resultId: resultId,
                userId: userId,
                questionId: questionId,
                score: 5,
                reviewNeededAnswer: "Some answer",
                reviewNeededResult: AnswerResult.Correct
            );

            dbContext.QuestionResults.Add(reviewNeededQuestionResult);
            dbContext.SaveChanges();

            // Act
            var result = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, resultId, questionId, 2).Result;

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().BeEquivalentTo(Error.ConditionNotMet);

            dbContext.QuestionResults.RemoveRange(dbContext.QuestionResults);
            dbContext.SaveChanges();
        }


        [Fact]
        public void Given_ZeroScore_When_UpdateQuestionResultIsCalled_Then_WrongAnswerIsStoredInDatabase()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");
            var resultId = Guid.NewGuid();

            var reviewNeededQuestionResult = new ReviewNeededQuestionResult(
                resultId: resultId,
                userId: userId,
                questionId: questionId,
                score: 5,
                reviewNeededAnswer: "Some answer",
                reviewNeededResult: AnswerResult.Pending
            );

            dbContext.QuestionResults.Add(reviewNeededQuestionResult);
            dbContext.SaveChanges();

            // Act
            var result = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, resultId, questionId, 0).Result;

            // Assert
            result.IsSuccess.Should().BeTrue();
            var updatedResult = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, resultId, questionId).Result;
            updatedResult.Should().NotBeNull();
            updatedResult.Should().BeOfType<ReviewNeededQuestionResult>();
            ((ReviewNeededQuestionResult)updatedResult).ReviewNeededResult.Should().Be(AnswerResult.Wrong);

            dbContext.QuestionResults.RemoveRange(dbContext.QuestionResults);
            dbContext.SaveChanges();
        }

        [Fact]
        public void Given_PartialScore_When_UpdateQuestionResultIsCalled_Then_PartiallyCorrectAnswerIsStoredInDatabase()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");
            var resultId = Guid.NewGuid();

            var reviewNeededQuestionResult = new ReviewNeededQuestionResult(
                resultId: resultId,
                userId: userId,
                questionId: questionId,
                score: 5,
                reviewNeededAnswer: "Some answer",
                reviewNeededResult: AnswerResult.Pending
            );

            dbContext.QuestionResults.Add(reviewNeededQuestionResult);
            dbContext.SaveChanges();

            // Act
            var result = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, resultId, questionId, 1).Result;

            // Assert
            result.IsSuccess.Should().BeTrue();
            var updatedResult = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, resultId, questionId).Result;
            updatedResult.Should().NotBeNull();
            updatedResult.Should().BeOfType<ReviewNeededQuestionResult>();
            ((ReviewNeededQuestionResult)updatedResult).ReviewNeededResult.Should().Be(AnswerResult.PartiallyCorrect);

            dbContext.QuestionResults.RemoveRange(dbContext.QuestionResults);
            dbContext.SaveChanges();
        }

        [Fact]
        public void Given_MaxScore_When_UpdateQuestionResultIsCalled_Then_CorrectAnswerIsStoredInDatabase()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");
            var resultId = Guid.NewGuid();

            var reviewNeededQuestionResult = new ReviewNeededQuestionResult(
                resultId: resultId,
                userId: userId,
                questionId: questionId,
                score: 5,
                reviewNeededAnswer: "Some answer",
                reviewNeededResult: AnswerResult.Pending
            );

            dbContext.QuestionResults.Add(reviewNeededQuestionResult);
            dbContext.SaveChanges();

            // Act
            var result = UpdateQuestionResultExtension.UpdateQuestionResultAsync(dbContext, resultId, questionId, 5).Result;

            // Assert
            result.IsSuccess.Should().BeTrue();
            var updatedResult = FetchQuestionResultExtension.FetchQuestionResultAsync(dbContext, resultId, questionId).Result;
            updatedResult.Should().NotBeNull();
            updatedResult.Should().BeOfType<ReviewNeededQuestionResult>();
            ((ReviewNeededQuestionResult)updatedResult).ReviewNeededResult.Should().Be(AnswerResult.Correct);

            dbContext.QuestionResults.RemoveRange(dbContext.QuestionResults);
            dbContext.SaveChanges();
        }
    }
}