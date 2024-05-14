using System.Net;
using FluentAssertions;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Features;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;
using OQS.CoreWebAPI.Tests.SetUp;

namespace OQS.CoreWebAPI.Tests
{

    public class EmailSenderTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task SendEmail_WhenCalled_ReturnsSendEmailResponse()
        {
            // Arrange
            var RecipientEmail = "galatanuemilia14@gmail.com";
            var QuizId = Guid.Parse("00000000-0000-0000-0002-000000000001");
            var UserId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var requestUri = $"api/email/send?" +
                $"recipientEmail={RecipientEmail}&" +
                $"quizId={QuizId}&" +
                $"userId={UserId}";


            // Act
            var result = await Client.GetAsync(requestUri);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
