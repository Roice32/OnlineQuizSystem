using FluentAssertions;
using Newtonsoft.Json;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
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
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000002");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");
            var finalScore = 1;
            
            var requestUri1 = $"api/quizResults/reviewResult?" +
                $"userId=&" +
                $"quizId={quizId}&" +
                $"questionId={questionId}&" +
                $"finalScore={finalScore}";
            var requestUri2 = $"api/quizResults/reviewResult?" +
                $"userId={userId}&" +
                $"quizId=&" +
                $"questionId={questionId}&" +
                $"finalScore={finalScore}";
            var requestUri3 = $"api/quizResults/reviewResult?" +
                $"userId={userId}&" +
                $"quizId={quizId}&" +
                $"questionId=&" +
                $"finalScore={finalScore}";
            var requestUri4 = $"api/quizResults/reviewResult?" +
                $"userId={userId}&" +
                $"quizId={quizId}&" +
                $"questionId={questionId}&" +
                $"finalScore=";

            // Act
            var result1 = await Client.PutAsync(requestUri1, null);
            var result2 = await Client.PutAsync(requestUri2, null);
            var result3 = await Client.PutAsync(requestUri3, null);
            var result4 = await Client.PutAsync(requestUri4, null);

            // Assert
            result1.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString1 = await result1.Content.ReadAsStringAsync();
            resultString1.Should().Contain("Failed to bind parameter \"Guid userId\" from \"\"");

            result2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString2 = await result2.Content.ReadAsStringAsync();
            resultString2.Should().Contain("Failed to bind parameter \"Guid quizId\" from \"\"");

            result3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString3 = await result3.Content.ReadAsStringAsync();
            resultString3.Should().Contain("Failed to bind parameter \"Guid questionId\" from \"\"");

            result4.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString4 = await result4.Content.ReadAsStringAsync();
            resultString4.Should().Contain("Failed to bind parameter \"float finalScore\" from \"\"");
        }
        
        [Fact]    
        public async Task Given_InvalidReviewCommand_When_ReviewAnswerHandlerIsCalled_Then_SpecificErrorIsReturned()
        {
            // Arrange
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");
            var requestUri1 = $"api/quizResults/reviewResult?" +
                $"userId={userId}&" +
                $"quizId={Guid.Parse("00000000-0000-0000-0002-000000000002")}&" +
                $"questionId={questionId}&" +
                $"finalScore={1}";
            var requestUri2 = $"api/quizResults/reviewResult?" +
                $"userId={Guid.Parse("00000000-0000-0000-0001-000000000002")}&" +
                $"quizId={quizId}&" +
                $"questionId={questionId}&" +
                $"finalScore={1}";
            var requestUri3 = $"api/quizResults/reviewResult?" +
                $"userId={userId}&" +
                $"quizId={quizId}&" +
                $"questionId={questionId}&" +
                $"finalScore={-1}";
            var requestUri4 = $"api/quizResults/reviewResult?" +
                $"userId={Guid.Parse("00000000-0000-0000-0001-000000000001")}&" +
                $"quizId={Guid.Parse("00000000-0000-0000-0002-000000000001")}&" +
                $"questionId={Guid.Parse("00000000-0000-0000-0003-000000000001")}&" +
                $"finalScore={1}";
            var requestUri5 = $"api/quizResults/reviewResult?" +
                $"userId={userId}&" +
                $"quizId={quizId}&" +
                $"questionId={Guid.Parse("00000000-0000-0000-0003-000000000007")}&" +
                $"finalScore={1}";

            // Act
            var result1 = await Client.PutAsync(requestUri1, null);
            var result2 = await Client.PutAsync(requestUri2, null);
            var result3 = await Client.PutAsync(requestUri3, null);
            var result4 = await Client.PutAsync(requestUri4, null);
            var result5 = await Client.PutAsync(requestUri5, null);

            // Assert
            result1.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString1 = await result1.Content.ReadAsStringAsync();
            resultString1.Should().Contain("QuizId and QuestionId correspondence does not exist.");

            result2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString2 = await result2.Content.ReadAsStringAsync();
            resultString2.Should().Contain(Error.NullValue.Message);

            result3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString3 = await result3.Content.ReadAsStringAsync();
            resultString3.Should().Contain(Error.OutOfBoundsValue.Message);

            result4.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString4 = await result4.Content.ReadAsStringAsync();
            resultString4.Should().Contain(Error.InvalidType.Message);

            result5.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var resultString5 = await result5.Content.ReadAsStringAsync();
            resultString5.Should().Contain(Error.ConditionNotMet.Message);
        }

        [Fact]
        public async Task Given_ValidReviewCommand_When_ReviewAnswerHandlerIsCalled_Then_ResultIsUpdatedInDatabaseAndReturned()
        {
            // Arrange
            var userId = Guid.Parse("00000000-0000-0000-0001-000000000003");
            var quizId = Guid.Parse("00000000-0000-0000-0002-000000000003");
            var questionId = Guid.Parse("00000000-0000-0000-0003-000000000008");
            var finalScore = 1;

            var requestUri = $"api/quizResults/reviewResult?" +
                $"userId={userId}&" +
                $"quizId={quizId}&" +
                $"questionId={questionId}&" +
                $"finalScore={finalScore}";

            // Act
            var result = await Client.PutAsync(requestUri, null);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            var resultString = await result.Content.ReadAsStringAsync();
            var resultObject = JsonConvert.DeserializeObject<ReviewAnswerResponse>(resultString);
            
            resultObject.UpdatedQuizResultHeader.Should().NotBeNull();
            resultObject.UpdatedQuizResultHeader.Score.Should().Be(1);
            resultObject.UpdatedQuizResultHeader.ReviewPending.Should().BeFalse();

            resultObject.UpdatedQuestionResult.Should().NotBeNull();
            resultObject.UpdatedQuestionResult.Score.Should().Be(1);
            resultObject.UpdatedQuestionResult.ReviewNeededResult.Should().Be(AnswerResult.PartiallyCorrect);
        }
    }
}
