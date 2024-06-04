using FluentAssertions;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;
using Xunit;

namespace OQS.CoreWebAPI.Tests.ResultsAndStatisticsTests
{
    public class QuizResultTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_IdForNonexistentQuizResultHeader_When_GetCreatedQuizStatsHandlerIsCalled_Then_NullValueIsReturned()
        {
            // Arrange
            var resultId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var requestUri = "api/quizResults/getQuizResult/" + resultId;

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Given_IdForUnauthorized_When_GetQuizResultHandlerIsCalled_Then_UnauthorizedIsReturned()
        {
            // Arrange
            var resultId = Guid.Parse("00000000-0000-0000-0004-000000000001");
            var requestUri = "api/quizResults/getQuizResult/" + resultId;

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task When_GetQuizResultHandlerIsCalled_Then_CorrectStatsAreReturned()
        {
            var resultId = Guid.Parse("00000000-0000-0000-0004-000000000002");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var requestUri = "api/quizResults/getQuizResult/" + resultId;


            // Act
            var result = await Client.GetAsync(requestUri);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultString = await result.Content.ReadAsStringAsync();
            var resultObject = JsonConvert
                .DeserializeObject<GetQuizResultResponse>(resultString,
                    new CustomJsonDeserializer());

            resultObject.QuizResultHeader.QuizId.Should().Be(quizId);
            resultObject.QuizResultHeader.UserId.Should().Be(userId);
            resultObject.QuizResultHeader.Score.Should().Be(0);

        }
    }
}