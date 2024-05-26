using FluentAssertions;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;

namespace OQS.CoreWebAPI.Tests.ResultsAndStatisticsTests
{
    public class TakenQuizzesHistoryTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_IdForNonexistentUser_When_GetTakenQuizzesHistory_Then_NullValueIsReturned()
        {
            // Arrange
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000009");
            var requestUri = "api/quizResults/getTakenQuizzesHistory/" + userId;

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task When_GetTakenQuizzesHistoryIsCalled_Then_CorrectStatsAreReturned()
        {
            // Arrange
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var requestUri = "api/quizResults/getTakenQuizzesHistory/" + userId;

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultString = await result.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<GetTakenQuizzesHistoryResponse>(resultString);

            resultObject.QuizNames.Should().HaveCount(1);
            resultObject.QuizResultHeaders.Should().HaveCount(1);
            resultObject.QuizNames.Should().ContainKey(Guid.Parse("00000000-0000-0000-0002-000000000001"));
            resultObject.QuizNames[Guid.Parse("00000000-0000-0000-0002-000000000001")].Should().Be("Quiz1");
            resultObject.QuizResultHeaders[0].QuizId.Should().Be(Guid.Parse("00000000-0000-0000-0002-000000000001"));

        }

        [Fact]
        public async Task Given_UserDidNotTakeAnyQuizzes_When_GetCreatedQuizStatsHandlerIsCalled_Then_EmptyQuizNamesAndQuizResultHeadersAreReturned()
        {
            // Arrange
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000002");
            var requestUri = "api/quizResults/getTakenQuizzesHistory/" + userId;

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultString = await result.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<GetTakenQuizzesHistoryResponse>(resultString);

            resultObject.QuizNames.Should().BeEmpty();
            resultObject.QuizResultHeaders.Should().BeEmpty();
        }
    }
}