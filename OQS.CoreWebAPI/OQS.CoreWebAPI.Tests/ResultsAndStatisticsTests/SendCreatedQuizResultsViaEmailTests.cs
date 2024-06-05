using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;
using Newtonsoft.Json;

namespace OQS.CoreWebAPI.Tests.ResultsAndStatisticsTests
{
    public class SendCreatedQuizStatsViaEmailTests : ApplicationContextForTesting
    {
        // DISCLAIMER!
        // Some tests may not pass on some machines based on date format.
        private async Task<string> GenerateValidToken(HttpClient client)
        {
            var command = new Authentication.Command
            {
                Username = "User2",
                Password = "User@123"
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
        public async Task Given_ValidParameters_When_SendCreatedQuizStatsViaEmailIsCalled_Then_StatusCodeOkIsReturned()
        {
            // Arrange
            var recipientEmail = "test@yahoo.com";
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");
            var startDate = DateTime.UtcNow.AddDays(-7);
            var formattedStartDate = startDate.ToString("MM/dd/yyyy");
            var endDate = DateTime.UtcNow;
            var formattedEndDate = endDate.ToString("MM/dd/yyyy");
            var requestUri = $"api/email/sendCreatedQuizStatsViaEmail?" +
                             $"quizId={quizId}&" +
                             $"recipientEmail={recipientEmail}&" +
                             $"startDate={formattedStartDate}&" +
                             $"endDate={formattedEndDate}";

            var application = Application.CreateClient();
            var tokenJson = GenerateValidToken(application).Result;
            var token = JsonConvert.DeserializeObject<Token>(tokenJson);
            application.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.token}");

            // Act
            var result = await application.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Given_StartDateGreaterThanEndDate_When_SendCreatedQuizStatsViaEmailIsCalled_Then_BadRequestIsReturned()
        {
            // Arrange
            var recipientEmail = "writeYourEmailHere";
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");
            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow.AddDays(-7);
            var requestUri = $"api/email/sendCreatedQuizStatsViaEmail?" +
                             $"quizId={quizId}&" +
                             $"recipientEmail={recipientEmail}&" +
                             $"startDate={startDate}&" +
                             $"endDate={endDate}";

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}