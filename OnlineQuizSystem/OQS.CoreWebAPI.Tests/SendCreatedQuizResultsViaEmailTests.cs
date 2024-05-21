using System.Net;
using FluentAssertions;
using OQS.CoreWebAPI.Tests.SetUp;

namespace OQS.CoreWebAPI.Tests
{
    public class SendCreatedQuizResultsViaEmailTests : ApplicationContextForTesting
    {
        // DISCLAIMER!
        // Some tests may not pass on some machines based on date format.
        [Fact]
        public async Task Given_ValidParameters_When_SendCreatedQuizStatsViaEmailIsCalled_Then_StatusCodeOkIsReturned()
        {
            // Arrange
            var recipientEmail = "writeValidEmailHere";
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

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Given_StartDateGreaterThanEndDate_When_SendCreatedQuizStatsViaEmailIsCalled_Then_BadRequestIsReturned()
        {
            // Arrange
            var recipientEmail = "writeValidEmailHere";
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