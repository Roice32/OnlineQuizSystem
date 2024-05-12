using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Extensions;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Features;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;

namespace OQS.CoreWebAPI.Tests
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
            int timeElapsed = 10;

            var requestUri1 = $"api/quizResults/processQuizSubmission?" +
                $"quizId=&" +
                $"takenBy={takenBy}&" +
                $"questionAnswerPairsJSON={questionAnswerPairsJson}&" +
                $"timeElapsed={timeElapsed}";

            var requestUri2 = $"api/quizResults/processQuizSubmission?" +
                $"quizId={quizId}&" +
                $"takenBy=&" +
                $"questionAnswerPairsJSON={questionAnswerPairsJson}&" +
                $"timeElapsed={timeElapsed}";

            var requestUri3 = $"api/quizResults/processQuizSubmission?" +
                $"quizId={quizId}&" +
                $"takenBy={takenBy}&" +
                $"questionAnswerPairsJSON=&" +
                $"timeElapsed={timeElapsed}";

            
            var requestUri4 = $"api/quizResults/processQuizSubmission?" +
                $"quizId={quizId}&" +
                $"takenBy={takenBy}&" +
                $"questionAnswerPairsJSON={questionAnswerPairsJson}&" +
                $"timeElapsed=";

            // Act
            var result1 = await Client.PostAsync(requestUri1, null);
            var result2 = await Client.PostAsync(requestUri2, null);
            var result3 = await Client.PostAsync(requestUri3, null);
            var result4 = await Client.PostAsync(requestUri4, null);

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

            result4.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString4 = await result4.Content.ReadAsStringAsync();
            resultString4.Should().Contain("Failed to bind parameter \"int timeElapsed\" from \"\"");
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
                new TrueFalseQAPair(Guid.Parse("00000000-0000-0000-0003-000000000007"), false),
            ];
            string questionAnswerPairsJson = JsonConvert.SerializeObject(questionAnswerPairs);
            int timeElapsed = 1;

            var requestUri = $"api/quizResults/processQuizSubmission?" +
                $"quizId={quizId}&" +
                $"takenBy={takenBy}&" +
                $"questionAnswerPairsJSON={questionAnswerPairsJson}&" +
                $"timeElapsed={timeElapsed}";

            // Act
            var result = await Client.PostAsync(requestUri, null);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultHeader = await FetchQuizResultHeaderExtension
                .FetchQuizResultHeaderAsync(dbContext, quizId, takenBy);
            resultHeader.IsSuccess.Should().BeTrue();
            resultHeader.Value.QuizId.Should().Be(quizId);
            resultHeader.Value.UserId.Should().Be(takenBy);
            resultHeader.Value.CompletionTime.Should().Be(timeElapsed);
            // Since Quizzes table does not contain questions, this result cannot update
            //resultHeader.Value.Score.Should().Be(2);
        }
    }
}