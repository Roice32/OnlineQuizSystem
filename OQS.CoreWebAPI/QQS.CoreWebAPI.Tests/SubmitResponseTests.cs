using System;
using System.Collections.Generic;
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
        private string GenerateJwtToken(string userId)
        {
            // Assuming you have a method to generate a valid JWT token.
            // Replace this with your actual JWT token generation logic.
            return "eyJhbGciOi"; // Simulated JWT token
        }

        [Fact]
        public async Task SubmitResponse_ReturnsOk()
        {
            var client = Application.CreateClient();
            var token = GenerateJwtToken("5b048913-5df0-429f-a42b-051904672e4d");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450");
            var userId = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d");

            var newResponse = new SubmitResponseRequest
            {
                UserId = userId.ToString(),
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

            var response = await client.PostAsJsonAsync($"api/active-quizzes/{activeQuizId}", newResponse);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Result<string>>(jsonString);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task SubmitResponse_InvalidUserId_ReturnsUnauthorized()
        {
            var client = Application.CreateClient();
            var token = GenerateJwtToken("5b048913-5df0-429f-a42b-051904672e4d");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450");
            var invalidUserId = Guid.Empty;

            var newResponse = new SubmitResponseRequest
            {
                UserId = invalidUserId.ToString(),
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

            var response = await client.PostAsJsonAsync($"api/active-quizzes/{activeQuizId}", newResponse);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            var result = await response.Content.ReadFromJsonAsync<Result<string>>();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("Unauthorized");
        }

        [Fact]
        public async Task SubmitResponse_PastDeadline_ReturnsBadRequest()
        {
            var client = Application.CreateClient();
            var token = GenerateJwtToken("5b048913-5df0-429f-a42b-051904672e4d");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972451");
            var userId = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d");

            var newResponse = new SubmitResponseRequest
            {
                UserId = userId.ToString(),
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

            var response = await client.PostAsJsonAsync($"api/active-quizzes/{activeQuizId}", newResponse);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var result = await response.Content.ReadFromJsonAsync<Result<string>>();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("BadRequest");
            result.Error.Message.Should().Be("Submissions are closed.");
        }
    }
}
