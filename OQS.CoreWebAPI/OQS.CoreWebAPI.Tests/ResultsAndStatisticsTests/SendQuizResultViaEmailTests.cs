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
            var RecipientEmail = "writeValidEmailHere";
            var QuizId = Guid.Parse("00000000-0000-0000-0002-000000000001");
            var UserId = Guid.Parse("00000000-0000-0000-0001-000000000001");
            var requestUri = $"api/email/sendQuizResultViaEmail?" +
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