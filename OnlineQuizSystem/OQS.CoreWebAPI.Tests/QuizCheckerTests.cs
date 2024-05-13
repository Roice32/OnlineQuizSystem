using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.Checkers;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;

namespace OQS.CoreWebAPI.Tests
{
    public class QuizCheckerTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_IdForNonexistentQuiz_When_CheckQuizIsCalled_Then_NullValueIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var quizSubmission = new QuizSubmission
            (
                quizId: Guid.Parse("00000000-0000-0000-0002-000000000009"),
                takenBy: Guid.Parse("00000000-0000-0000-0001-000000000001"),
                questionAnswerPairs: [],
                timeElapsed: 2
            );

            // Act
            var result = await QuizChecker.CheckQuizAsync(quizSubmission, dbContext);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.NullValue);
        }

        [Fact]
        public async Task Given_IdForQuizAlreadyTakenByUser_When_CheckQuizIsCalled_Then_DuplicateEntityIsReturned()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var quizSubmission = new QuizSubmission
            (
                quizId: Guid.Parse("00000000-0000-0000-0002-000000000001"),
                takenBy: Guid.Parse("00000000-0000-0000-0001-000000000001"),
                questionAnswerPairs: [],
                timeElapsed: 2
            );

            // Act
            var result = await QuizChecker.CheckQuizAsync(quizSubmission, dbContext);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be(Error.DuplicateEntity);
        }

        [Fact]
        public async Task Given_NoAnswers_WhenCheckQuizIsCalled_Then_StoredQuizResultHasScoreZeroAndNotAnsweredResults()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000002");
            var quizSubmission = new QuizSubmission
            (
                quizId: quizId,
                takenBy: userId,
                questionAnswerPairs: [],
                timeElapsed: 2
            );

            // Act
            var result = await QuizChecker.CheckQuizAsync(quizSubmission, dbContext);
            var questionIds = await dbContext.Questions
                .Where(q => q.QuizId == quizId)
                .Select(q => q.Id)
                .ToListAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            var quizResult = await FetchQuizResultHeaderExtension.FetchQuizResultHeaderAsync(dbContext, quizId, userId);
            quizResult.IsSuccess.Should().BeTrue();
            quizResult.Value.Score.Should().Be(0);
            quizResult.Value.ReviewPending.Should().BeFalse();

            var questionResults = await dbContext.QuestionResults
                .Where(qr => qr.UserId == userId
                    && questionIds.Contains(qr.QuestionId))
                .ToListAsync();

            questionResults.Should().HaveCount(5);
            foreach(var questionResult in questionResults)
            {
                questionResult.Score.Should().Be(0);
                if (questionResult is TrueFalseQuestionResult trueFalseQuestionResult)
                {
                    trueFalseQuestionResult.TrueFalseAnswerResult.Should().Be(AnswerResult.NotAnswered);
                }
                else if (questionResult is ChoiceQuestionResult choiceQuestionResult)
                {
                    choiceQuestionResult.PseudoDictionaryChoicesResults.Should().Be(
                        JsonConvert.SerializeObject(new Dictionary<string, AnswerResult>()));
                }
                else if (questionResult is WrittenAnswerQuestionResult writtenAnswerQuestionResult)
                {
                    writtenAnswerQuestionResult.WrittenAnswerResult.Should().Be(AnswerResult.NotAnswered);
                }
                else if (questionResult is ReviewNeededQuestionResult reviewNeededQuestionResult)
                {
                    reviewNeededQuestionResult.ReviewNeededResult.Should().Be(AnswerResult.NotAnswered);
                }
            }
        }
    }
}
