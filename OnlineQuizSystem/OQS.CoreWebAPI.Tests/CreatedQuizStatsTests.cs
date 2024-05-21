using FluentAssertions;
using Newtonsoft.Json;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;

namespace OQS.CoreWebAPI.Tests
{
    public class CreatedQuizStatsTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_IdForNonexistentQuiz_When_GetCreatedQuizStatsHandlerIsCalled_Then_NullValueIsReturned()
        {
            // Arrange
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000009");
            var requestUri = "api/quizResults/getCreatedQuizStats/" + quizId;

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task When_GetCreatedQuizStatsHandlerIsCalled_Then_CorrectStatsAreReturned()
        {
            // Arrange
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");
            var requestUri = "api/quizResults/getCreatedQuizStats/" + quizId;

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultString = await result.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<GetCreatedQuizStatsResponse>(resultString);

            resultObject.QuizName.Should().Be("Quiz1");
            resultObject.UserNames.Should().HaveCount(1);
            resultObject.QuizResultHeaders.Should().HaveCount(1);
            resultObject.QuizResultHeaders[0].QuizId.Should().Be(quizId);
            resultObject.QuizResultHeaders[0].Score.Should().Be(13);
        }

        [Fact]
        public async Task Given_NoUserTookQuiz_When_GetCreatedQuizStatsHandlerIsCalled_Then_EmptyUserNamesAndQuizResultHeadersAreReturned()
        {
            // Arrange
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");
            var requestUri = "api/quizResults/getCreatedQuizStats/" + quizId;

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultString = await result.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<GetCreatedQuizStatsResponse>(resultString);

            resultObject.QuizName.Should().Be("Quiz2");
            resultObject.UserNames.Should().BeEmpty();
            resultObject.QuizResultHeaders.Should().BeEmpty();
        }
    }
}
