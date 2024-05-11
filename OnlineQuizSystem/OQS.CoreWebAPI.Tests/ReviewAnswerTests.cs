using FluentAssertions;
using Newtonsoft.Json;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Features;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;
using System.Text;

namespace OQS.CoreWebAPI.Tests
{
    public class ReviewAnswerTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task Given_AnyEmptyFieldInCommad_When_ReviewAnswerHandlerIsCalled_Then_ValidationFails()
        {
            // Arrange
            var requestUri = "api/quizResults/reviewResult";
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");
            var finalScore = 1;

            var command1 = new ReviewAnswer.Command
            {
                UserId = Guid.Empty,
                QuizId = quizId,
                QuestionId = questionId,
                FinalScore = finalScore
            };
            var content1 = new StringContent(JsonConvert.SerializeObject(command1), Encoding.UTF8, "application/json");
            
            var command2 = new ReviewAnswer.Command
            {
                UserId = userId,
                QuizId = Guid.Empty,
                QuestionId = questionId,
                FinalScore = finalScore
            };
            var content2 = new StringContent(JsonConvert.SerializeObject(command2), Encoding.UTF8, "application/json");

            var command3 = new ReviewAnswer.Command
            {
                UserId = userId,
                QuizId = quizId,
                QuestionId = Guid.Empty,
                FinalScore = finalScore
            };
            var content3 = new StringContent(JsonConvert.SerializeObject(command3), Encoding.UTF8, "application/json");

            // Act & Assert
            var result1 = await Client.PutAsync(requestUri, content1);
            result1.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result2 = await Client.PutAsync(requestUri, content2);
            result2.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var result3 = await Client.PutAsync(requestUri, content3);
            result3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact]    
        /*public async Task Given_InvalidReviewCommand_Then_ReviewAnswerHandlerIsCalled_Then_SpecificErrorIsReturned()
        {
            // Arrange
            var requestUri = "api/quizResults/reviewResult";
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");

            var command1 = new ReviewAnswer.Command
            {
                UserId = Guid.Parse("00000000-0000-0000-0001-000000000009"),
                QuizId = quizId,
                QuestionId = questionId,
                FinalScore = 1
            };
            var content1 = new StringContent(JsonConvert.SerializeObject(command1), Encoding.UTF8, "application/json");

            var command2 = new ReviewAnswer.Command
            {
                UserId = userId,
                QuizId = quizId,
                QuestionId = questionId,
                FinalScore = -1
            };
            var content2 = new StringContent(JsonConvert.SerializeObject(command2), Encoding.UTF8, "application/json");

            var command3 = new ReviewAnswer.Command
            {
                UserId = userId,
                QuizId = quizId,
                QuestionId = questionId,
                FinalScore = 101
            };
            var content3 = new StringContent(JsonConvert.SerializeObject(command3), Encoding.UTF8, "application/json");

            // Act & Assert
            var result1 = await Client.PutAsync(requestUri, content1);
            result1.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString1 = await result1.Content.ReadAsStringAsync();
            resultString1.Should().Contain(Error.NullValue.Message);

            var result2 = await Client.PutAsync(requestUri, content2);
            result2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString2 = await result2.Content.ReadAsStringAsync();
            resultString2.Should().Contain(Error.OutOfBoundsValue.Message);

            var result3 = await Client.PutAsync(requestUri, content3);
            result3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString3 = await result3.Content.ReadAsStringAsync();
            resultString3.Should().Contain(Error.OutOfBoundsValue.Message);
        }*/
    }
}
