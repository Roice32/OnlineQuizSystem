using FluentAssertions;
using Newtonsoft.Json;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;

namespace OQS.CoreWebAPI.Tests
{
    public class QuizResultTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_IdForNonexistentQuizResultHeader_When_GetCreatedQuizStatsHandlerIsCalled_Then_NullValueIsReturned()
        {
            // Arrange
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");
            var requestUri = "api/quizResults/getQuizResult/" + userId + "/" + quizId;

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task When_GetQuizResultHandlerIsCalled_Then_CorrectStatsAreReturned()
        {
            // Arrange
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");
            var requestUri = "api/quizResults/getQuizResult/" + userId + "/" + quizId;

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultString = await result.Content.ReadAsStringAsync();
            // Deserialization fails because QuestionBase is abstract
            var resultObject = JsonConvert.DeserializeObject<GetQuizResultResponse>(resultString);

            resultObject.QuizResultHeader.QuizId.Should().Be(quizId);
            resultObject.QuizResultHeader.UserId.Should().Be(userId);
            resultObject.QuizResultHeader.Score.Should().Be(13);
            resultObject.QuizResultBody.Questions.Should().HaveCount(5);
        }
    }
}