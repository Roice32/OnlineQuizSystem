using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace OQS.CoreWebAPI.Tests.ResultsAndStatisticsTests
{
    public class TakenQuizzesHistoryTests : ApplicationContextForTesting
    {
        private async Task<string> GenerateValidToken(HttpClient client, string username, string password)
        {
            var command = new Authentication.Command
            {
                Username = username,
                Password = password
            };
            var response = client.PostAsJsonAsync("api/authentication", command);
            if (response.Result.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Authentication failed");
            }
            var token = await response.Result.Content.ReadAsStringAsync();
            return token;
        }

        public class Token
        {
            public string token { get; set; }
        }

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
            var application = Application.CreateClient();
            var tokenJson = GenerateValidToken(application, "User1", "User@123").Result;
            var Token = JsonConvert.DeserializeObject<Token>(tokenJson);
            application.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token.token}");

            // Act
            var result = await application.GetAsync("api/quizResults/getTakenQuizzesHistory/00000000-0000-0000-0001-000000000001");

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

            var application = Application.CreateClient();
            var tokenJson = GenerateValidToken(application, "User2", "User@123").Result;
            var token = JsonConvert.DeserializeObject<Token>(tokenJson);
            application.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.token}");

            // Act
            var result = await application.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultString = await result.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<GetTakenQuizzesHistoryResponse>(resultString);

            resultObject.QuizNames.Should().BeEmpty();
            resultObject.QuizResultHeaders.Should().BeEmpty();
        }
    }
}