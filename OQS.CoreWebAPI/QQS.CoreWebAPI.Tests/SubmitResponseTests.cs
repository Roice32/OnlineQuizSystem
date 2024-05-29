using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace QQS.CoreWebAPI.Tests
{
    public class SubmitResponseTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task SubmitResponse_ReturnsOk()
        {
            // Arrange
            var client = Application.CreateClient();

            // Add JWT token to request headers
            var token = "your_jwt_token_here"; // Replace with your JWT token
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450");

            var newResponse = new SubmitResponseRequest
            {
                // No need to specify UserId
                ActiveQuizId = activeQuizId,
                Answers = new List<Answer>
                {
                    new Answer
                    {
                        QuestionId = Guid.NewGuid(),
                        Type = QuestionType.SingleChoice,
                        SingleChoiceAnswer = "A"
                    }
                }
            };

            // Act
            var response = await client.PostAsJsonAsync($"api/active-quizzes/{activeQuizId}", newResponse);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Result<string>>(jsonString);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task SubmitResponse_InvalidUserId_ReturnsBadRequest()
        {
            // Arrange
            var client = Application.CreateClient();

            // Add JWT token to request headers
            var token = "your_jwt_token_here"; // Replace with your JWT token
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450");

            var newResponse = new SubmitResponseRequest
            {
                // No need to specify UserId
                ActiveQuizId = activeQuizId,
                Answers = new List<Answer>
                {
                    new Answer
                    {
                        QuestionId = Guid.NewGuid(),
                        Type = QuestionType.SingleChoice,
                        SingleChoiceAnswer = "A"
                    }
                }
            };

            // Act
            var response = await client.PostAsJsonAsync($"api/active-quizzes/{activeQuizId}", newResponse);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<Result<string>>();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull(); // Ensure that an error object is present
            result.Error.Code.Should().Be("BadRequest");
            ;
        }

        [Fact]
        public async Task SubmitResponse_PastDeadline_ReturnsBadRequest()
        {
            // Arrange
            var client = Application.CreateClient();

            // Add JWT token to request headers
            var token = "your_jwt_token_here"; // Replace with your JWT token
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972451");

            var newResponse = new SubmitResponseRequest
            {
                // No need to specify UserId
                ActiveQuizId = activeQuizId,
                Answers = new List<Answer>
                {
                    new Answer
                    {
                        QuestionId = Guid.NewGuid(),
                        Type = QuestionType.SingleChoice,
                        SingleChoiceAnswer = "A"
                    }
                }
            };

            // Act
            var response = await client.PostAsJsonAsync($"api/active-quizzes/{activeQuizId}", newResponse);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<Result<string>>();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("BadRequest");
            result.Error.Message.Should().Be("Submissions are closed.");
        }
    }
}
