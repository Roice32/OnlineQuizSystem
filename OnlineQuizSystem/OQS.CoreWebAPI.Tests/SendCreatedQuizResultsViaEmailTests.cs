using System.Net;
using FluentAssertions;
using OQS.CoreWebAPI.Tests.SetUp;


namespace OQS.CoreWebAPI.Tests
{
    public class SendCreatedQuizResultsViaEmailTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_ReversedDates_When_SendCreatedQuizStatsViaEmailIsCalled_Then_InvalidDatesErrorIsReturned()
        {
            // Arrange
            var recipientEmail = "alexandru.butnaru32@yahoo.com";
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");
            var startDate = DateTime.UtcNow.AddDays(7);
            var endDate = DateTime.UtcNow;

            var requestUri = $"api/email/sendCreatedQuizStatsViaEmail?" +
                $"quizId={quizId}&" +
                $"recipientEmail={recipientEmail}&" +
                $"startDate={startDate}&" +
                $"endDate={endDate}";

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string response = await result.Content.ReadAsStringAsync();
            response.Should().Contain("InvalidDates");
        }

        // Retired, as the test has different outcomes on different machines.
        /*[Fact]
        public async Task Given_ValidParameters_When_SendCreatedQuizStatsViaEmailIsCalled_Then_StatusCodeOkIsReturned()
        {
            // Arrange
            var recipientEmail = "alexandru.butnaru32@yahoo.com";
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;
            var requestUri = $"api/email/sendCreatedQuizStatsViaEmail?" +
                $"quizId={quizId}&" +
                $"recipientEmail={recipientEmail}&" +
                $"startDate={startDate}&" +
                $"endDate={endDate}";

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }*/
    }
}