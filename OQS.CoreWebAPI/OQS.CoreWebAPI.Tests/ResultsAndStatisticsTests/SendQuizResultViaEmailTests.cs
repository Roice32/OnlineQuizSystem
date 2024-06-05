using System.Net;
using FluentAssertions;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace OQS.CoreWebAPI.Tests.ResultsAndStatisticsTests
{

    public class SendQuizResultViaEmailTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task SendEmail_WhenCalled_ReturnsSendEmailResponse()
        {
            // Arrange
            var RecipientEmail = "test@yahoo.com";
            var ResultId = Guid.Parse("00000000-0000-0000-0004-000000000002");
            var requestUri = $"api/email/sendQuizResultViaEmail?" +
                $"recipientEmail={RecipientEmail}&" +
                $"resultId={ResultId}";

            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}