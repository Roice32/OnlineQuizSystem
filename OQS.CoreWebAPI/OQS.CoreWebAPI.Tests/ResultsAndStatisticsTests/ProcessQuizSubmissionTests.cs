﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionAnswerPairs;
using OQS.CoreWebAPI.Extensions.ResultsAndStatistics.QuizResultHeaders;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;
using Xunit;

namespace OQS.CoreWebAPI.Tests.ResultsAndStatisticsTests
{
    public class ProcessQuizSubmissionTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_AnyEmptyFieldInCommad_When_ProccesQuizSubmissionHandlerIsCalled_Then_ValidationFails()
        {
            // Arange 
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");
            var takenBy = Guid.Parse("00000000-0000-0000-0001-000000000003");
            List<QuestionAnswerPairBase> questionAnswerPairs = [new WrittenQAPair(Guid.Parse("00000000-0000-0000-0003-000000000007"), "SomeReviewNeededAnswer")];
            string questionAnswerPairsJson = JsonConvert.SerializeObject(questionAnswerPairs);

            var requestUri1 = $"api/quizResults/processQuizSubmission?" +
                $"quizId=&" +
                $"takenBy={takenBy}&" +
                $"questionAnswerPairsJSON={questionAnswerPairsJson}&";

            var requestUri2 = $"api/quizResults/processQuizSubmission?" +
                $"quizId={quizId}&" +
                $"takenBy=&" +
                $"questionAnswerPairsJSON={questionAnswerPairsJson}&";

            var requestUri3 = $"api/quizResults/processQuizSubmission?" +
                $"quizId={quizId}&" +
                $"takenBy={takenBy}&" +
                $"questionAnswerPairsJSON=&";

            // Act
            var result1 = await Client.PostAsync(requestUri1, null);
            var result2 = await Client.PostAsync(requestUri2, null);
            var result3 = await Client.PostAsync(requestUri3, null);

            // Assert
            result1.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString1 = await result1.Content.ReadAsStringAsync();
            resultString1.Should().Contain("Failed to bind parameter \"Guid quizId\" from \"\"");

            result2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString2 = await result2.Content.ReadAsStringAsync();
            resultString2.Should().Contain("Failed to bind parameter \"Guid takenBy\" from \"\"");

            result3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString3 = await result3.Content.ReadAsStringAsync();
            resultString3.Should().Contain("QuestionAnswerPairs is required.");
        }

        [Fact]
        public async Task Given_ValidCommand_When_ProccesQuizSubmissionHandlerIsCalled_Then_ResultIsStoredInDatabase()
        {
            // Arange
            using var scope = Application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");
            var takenBy = Guid.Parse("00000000-0000-0000-0001-000000000003");
            List<QuestionAnswerPairBase> questionAnswerPairs =
            [
                new TrueFalseQAPair(Guid.Parse("00000000-0000-0000-0003-000000000006"), false),
            ];
            string questionAnswerPairsJson = JsonConvert.SerializeObject(questionAnswerPairs);

            var requestUri = $"api/quizResults/processQuizSubmission?" +
                $"quizId={quizId}&" +
                $"takenBy={takenBy}&" +
                $"questionAnswerPairsJSON={questionAnswerPairsJson}&";
            // Act
            var result = await Client.PostAsync(requestUri, null);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultHeader = await FetchQuizResultHeaderExtension
                .FetchQuizResultHeaderAsync(dbContext, quizId, takenBy);
            resultHeader.IsSuccess.Should().BeTrue();
            resultHeader.Value.QuizId.Should().Be(quizId);
            resultHeader.Value.UserId.Should().Be(takenBy);
            resultHeader.Value.Score.Should().Be(2);
        }
    }
}