using System;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;
using OQS.CoreWebAPI.Entities;
namespace QQS.CoreWebAPI.Tests
{
    public class SubmitResponseTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task SubmitResponse_ReturnsOk()
        {
            var client = Application.CreateClient();
            
            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450");
            var userId = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d");

            var newResponse = new SubmitResponseRequest
            {
                UserId = userId,
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
        public async Task SubmitResponse_InvalidUserId_ReturnsBadRequest()
        {
            var client = Application.CreateClient();
            
            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450");
            var invalidUserId = Guid.Empty;

            var newResponse = new SubmitResponseRequest
            {
                UserId = invalidUserId,
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
            
       
            var result = await response.Content.ReadFromJsonAsync<Result<string>>();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("BadRequest");
        }
        [Fact]
        public async Task SubmitResponse_PastDeadline_ReturnsBadRequest()
        {
            var client = Application.CreateClient();
            
            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972451");
            var userId = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d");

            var newResponse = new SubmitResponseRequest
            {
                UserId = userId,
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

            var response = await Client.PostAsJsonAsync($"api/active-quizzes/{activeQuizId}", newResponse);
            
        
            var result = await response.Content.ReadFromJsonAsync<Result<string>>();
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Code.Should().Be("BadRequest");
            result.Error.Message.Should().Be("Submissions are closed.");
        }
    }
}
