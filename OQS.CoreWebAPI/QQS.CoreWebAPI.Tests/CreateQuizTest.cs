using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace QQS.CoreWebAPI.Tests
{
    public class QuizCreationTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task CreateAndDeleteQuiz_ReturnsOkStatus()
        {
            var newQuiz = new CreateQuizRequest
            {
                Name = "Test Quiz",
                Description = "Test Description",
                ImageUrl = "http://example.com/image.jpg",
                Language = "English",
                CreatorId = Guid.NewGuid(),
                TimeLimitMinutes = 30
            };

            // Create the quiz
            var createResponse = await Client.PostAsJsonAsync("api/quizzes", newQuiz);

            // Assert
            createResponse
                .EnsureSuccessStatusCode(); // Check if the HTTP response indicates a successful operation (2xx code)
            Assert.Equal(HttpStatusCode.OK,
                createResponse.StatusCode); // Check if the HTTP response is of type OK (200 code)
        }
    }
}