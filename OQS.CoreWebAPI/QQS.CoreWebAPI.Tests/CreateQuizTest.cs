using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace OQS.CoreWebAPI.Tests
{
    public class QuizCreationTests : ApplicationContextForTesting
    {
        [Fact]
        public async Task CreateQuiz_ReturnsOkStatus()
        {
            var newQuiz = new CreateQuizRequest
            {
                Name = "TestQuiz5",
                Description = "Test Description",
                ImageUrl = "http://example.com/image.jpg",
                Language = "English",
                CreatorId = Guid.Parse("00000000-0000-1000-0005-000000000000"),  
                TimeLimitMinutes = 30
            };

            // Act
            var createResponse = await Client.PostAsJsonAsync("api/quizzes", newQuiz);

            // Assert
            createResponse
                .EnsureSuccessStatusCode(); // Check if the HTTP response indicates a successful operation (2xx code)
            Assert.Equal(HttpStatusCode.OK,
                createResponse.StatusCode); // Check if the HTTP response is of type OK (200 code)
        }
      

          [Fact]
          public async Task CreateQuiz_WithInvalidData_ReturnsBadRequest()
          {
              var newQuiz = new CreateQuizRequest
              {
                  // Missing required fields to simulate invalid data
              };

              // Act
              var createResponse = await Client.PostAsJsonAsync("api/quizzes", newQuiz);

              // Assert
              createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
              var responseContent = await createResponse.Content.ReadAsStringAsync();
              responseContent.Should().Contain("Name is required")
                               .And.Contain("Language is required")
                               .And.Contain("Creator ID is required")
                               .And.Contain("Time limit is required");
          }


        [Fact]
        public async Task Given_AnyEmptyFieldInCommand_When_CreateQuizHandlerIsCalled_Then_ValidationFails()
        {
            // Arrange
            var testCases = new[]
            {
            new CreateQuizRequest
            {
                Description = "Test Description",
                ImageUrl = "http://example.com/image.jpg",
                Language = "English",
                CreatorId = Guid.Parse("00000000-0000-0000-1005-000000000000"),
                TimeLimitMinutes = 30
            },
            new CreateQuizRequest
            {
                Name = "TestQuiz5",
                Description = "Test Description",
                ImageUrl = "http://example.com/image.jpg",
                Language = "",
                CreatorId = Guid.Parse("00000000-0000-0000-1005-000000000000"),
                TimeLimitMinutes = 30
            },
            new CreateQuizRequest
            {
                Name = "TestQuiz5",
                Description = "Test Description",
                ImageUrl = "http://example.com/image.jpg",
                Language = "English",
                CreatorId = Guid.Empty,
                TimeLimitMinutes = 30
            },
            new CreateQuizRequest
            {
                Name = "TestQuiz5",
                Description = "Test Description",
                ImageUrl = "http://example.com/image.jpg",
                Language = "English",
                CreatorId = Guid.Parse("00000000-0000-0000-1005-000000000000"),
                TimeLimitMinutes = 0
            }
        };

            foreach (var newQuiz in testCases)
            {
                // Act
                var createResponse = await Client.PostAsJsonAsync("api/quizzes", newQuiz);

                // Assert
                createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                var resultString = await createResponse.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(newQuiz.Name))
                {
                    resultString.Should().Contain("Name is required.");
                }
                else if (string.IsNullOrEmpty(newQuiz.Language))
                {
                    resultString.Should().Contain("Language is required.");
                }
                else if (newQuiz.CreatorId == Guid.Empty)
                {
                    resultString.Should().Contain("Creator ID is required.");
                }
                else if (newQuiz.TimeLimitMinutes <= 0)
                {
                    resultString.Should().Contain("Time limit must be greater than 0.");
                }
            }
        }

    }
}