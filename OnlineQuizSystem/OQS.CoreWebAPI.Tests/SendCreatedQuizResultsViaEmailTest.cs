using System.Net;
using FluentAssertions;
using OQS.CoreWebAPI.Tests.SetUp;


namespace OQS.CoreWebAPI.Tests
{
    public class SendCreatedQuizResultsViaEmailTest : ApplicationContextForTesting
    {
        [Fact]
        public async Task SendEmail_WhenCalled_ReturnsSendEmailResponse()
        {
            // Arrange
            var recipientEmail = "denisa.golache@gmail.com";
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000001");
            var startDate = DateTime.Now.AddDays(-7);
            var endDate = DateTime.Now;
            var requestUri = $"api/email/sendCreatedQuizStatsViaEmail?" +
                $"quizId={quizId}&" +
                $"recipientEmail={recipientEmail}&" +
                $"startDate={startDate}&" +
                $"endDate={endDate}";

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}