using FluentAssertions;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Contracts.ResultsAndStatistics;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace OQS.CoreWebAPI.Tests.ResultsAndStatisticsTests
{
    public class CreatedQuizStatsTests : ApplicationContextForTesting
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
        public async Task Given_IdForNonexistentQuiz_When_GetCreatedQuizStatsHandlerIsCalled_Then_NullValueIsReturned()
        {
            // Arrange
            var application = Application.CreateClient();
            var tokenJson = GenerateValidToken(application, "User2", "User@123").Result;
            var Token = JsonConvert.DeserializeObject<Token>(tokenJson);
            application.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token.token}");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000009");
            var requestUri = "api/quizResults/getCreatedQuizStats/" + quizId;

            // Act
            var result = await application.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task When_GetCreatedQuizStatsHandlerIsCalled_Then_CorrectStatsAreReturned()
        {
            // Arrange
            var application = Application.CreateClient();
            var tokenJson = GenerateValidToken(application, "User2", "User@123").Result;
            var Token = JsonConvert.DeserializeObject<Token>(tokenJson);
            application.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token.token}");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");
            var requestUri = "api/quizResults/getCreatedQuizStats/" + quizId;

            // Act
            var result = await application.GetAsync(requestUri);

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

        // Fiddling with the seeded data is needed to make this test pass
        [Fact]
        public async Task Given_NoUserTookQuiz_When_GetCreatedQuizStatsHandlerIsCalled_Then_EmptyUserNamesAndQuizResultHeadersAreReturned()
        {
            // Arrange
            var application = Application.CreateClient();
            var tokenJson = GenerateValidToken(application, "User1", "User@123").Result;
            var Token = JsonConvert.DeserializeObject<Token>(tokenJson);
            application.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token.token}");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");
            var requestUri = "api/quizResults/getCreatedQuizStats/" + quizId;

            // Act
            var result = await application.GetAsync(requestUri);

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
