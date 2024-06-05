using FluentAssertions;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace OQS.CoreWebAPI.Tests.ResultsAndStatisticsTests
{
    public class ReviewAnswerTests : ApplicationContextForTesting
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
        public async Task Given_AnyEmptyFieldInCommad_When_ReviewAnswerHandlerIsCalled_Then_ValidationFails()
        {
            // Arrange
            var application = Application.CreateClient();
            var tokenJson = GenerateValidToken(application, "User1", "User@123").Result;
            var Token = JsonConvert.DeserializeObject<Token>(tokenJson);
            application.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token.token}");
            var resultId = Guid.Parse("00000000-0000-0000-0004-000000000002");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");
            var finalScore = 1;

            var requestUri1 = $"api/quizResults/reviewResult?" +
                $"resultId=&" +
                $"questionId={questionId}&" +
                $"finalScore={finalScore}";
            var requestUri2 = $"api/quizResults/reviewResult?" +
                $"resultId={resultId}&" +
                $"questionId=&" +
                $"finalScore={finalScore}";
            var requestUri3 = $"api/quizResults/reviewResult?" +
                $"resultId={resultId}&" +
                $"questionId={questionId}&" +
                $"finalScore=";

            // Act
            var result1 = await application.PutAsync(requestUri1, null);
            var result2 = await application.PutAsync(requestUri2, null);
            var result3 = await application.PutAsync(requestUri3, null);

            // Assert
            result1.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString1 = await result1.Content.ReadAsStringAsync();
            resultString1.Should().Contain("Failed to bind parameter \"Guid resultId\" from \"\"");

            result2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString2 = await result2.Content.ReadAsStringAsync();
            resultString2.Should().Contain("Failed to bind parameter \"Guid questionId\" from \"\"");

            result3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString3 = await result3.Content.ReadAsStringAsync();
            resultString3.Should().Contain("Failed to bind parameter \"float finalScore\" from \"\"");
        }

        [Fact]
        public async Task Given_InvalidReviewCommand_When_ReviewAnswerHandlerIsCalled_Then_SpecificErrorIsReturned()
        {
            // Arrange
            var application = Application.CreateClient();
            var tokenJson = GenerateValidToken(application, "User1", "User@123").Result;
            var Token = JsonConvert.DeserializeObject<Token>(tokenJson);
            application.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token.token}");
            var resultId = Guid.Parse("00000000-0000-0000-0004-000000000002");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");
            var requestUri1 = $"api/quizResults/reviewResult?" +
                $"resultId={Guid.Parse("00000000-0000-0000-0004-000000000009")}&" +
                $"questionId={questionId}&" +
                $"finalScore={1}";
            var requestUri2 = $"api/quizResults/reviewResult?" +
                $"resultId={resultId}&" +
                $"questionId={Guid.Parse("00000000-0000-0000-0003-000000000010")}&" +
                $"finalScore={1}";
            var requestUri3 = $"api/quizResults/reviewResult?" +
                $"resultId={resultId}&" +
                $"questionId={questionId}&" +
                $"finalScore={-1}";
            var requestUri4 = $"api/quizResults/reviewResult?" +
                $"resultId={resultId}&" +
                $"questionId={Guid.Parse("00000000-0000-0000-0003-000000000007")}&" +
                $"finalScore={1}";

            // Act
            var result1 = await application.PutAsync(requestUri1, null);
            var result2 = await application.PutAsync(requestUri2, null);
            var result3 = await application.PutAsync(requestUri3, null);
            var result4 = await application.PutAsync(requestUri4, null);

            // Assert
            result1.StatusCode.Should().Be(HttpStatusCode.NotFound);

            result2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString2 = await result2.Content.ReadAsStringAsync();
            resultString2.Should().Be("\"Value cannot be null.\"");

            result3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString3 = await result3.Content.ReadAsStringAsync();
            resultString3.Should().Contain(Error.OutOfBoundsValue.Message);

            result4.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString4 = await result4.Content.ReadAsStringAsync();
            resultString4.Should().Contain(Error.ConditionNotMet.Message);
        }

        [Fact]
        public async Task Given_RequestNotFromQuizCreator_When_ReviewAnswerHandlerIsCalled_Then_UnauthorizedIsReturned()
        {
            // Arrange
            var application = Application.CreateClient();
            var tokenJson = GenerateValidToken(application, "User2", "User@123").Result;
            var Token = JsonConvert.DeserializeObject<Token>(tokenJson);
            application.DefaultRequestHeaders.Add("Authorization", $"Bearer {Token.token}");
            var resultId = Guid.Parse("00000000-0000-0000-0004-000000000002");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");
            var finalScore = 1;

            var requestUri = $"api/quizResults/reviewResult?" +
                $"resultId={resultId}&" +
                $"questionId={questionId}&" +
                $"finalScore={finalScore}";

            // Act
            var result = await Client.PutAsync(requestUri, null);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        }
    }
}
