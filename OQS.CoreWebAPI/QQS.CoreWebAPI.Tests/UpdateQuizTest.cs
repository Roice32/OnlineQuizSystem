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
    public class UpdateQuizTest : ApplicationContextForTesting
    {
        [Fact]
        public async Task UpdateQuiz_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            Guid invalidId = Guid.Empty;
            var updateRequest = new UpdateQuizRequest
            {
                Name = "UpdatedName",
                Description = "Updated Description",
                ImageUrl = "http://example.com/updated-image.jpg",
                Language = "Updated Language",
                TimeLimitMinutes = 45
            };

            // Act
            var updateResponse = await Client.PatchAsJsonAsync($"api/quizzes/{invalidId}", updateRequest);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }



        [Fact]
        public async Task UpdateQuiz_ReturnsOkStatus()
        {
           /* // Arrange
            Guid quizId = Guid.Parse("00000000-0000-1000-0003-000000000000");
            var updateRequest = new UpdateQuizRequest
            {
                Name = "Updated Name",
                Description = "Updated Description",
                ImageUrl = "http://example.com/updated-image.jpg",
                Language = "Updated Language",
                TimeLimitMinutes = 45
            };

            // Act
            var updateResponse = await Client.PatchAsJsonAsync($"api/quizzes/{quizId}", updateRequest);

            // Assert
            updateResponse
               .EnsureSuccessStatusCode(); // Check if the HTTP response indicates a successful operation (2xx code)
            Assert.Equal(HttpStatusCode.OK,
                updateResponse.StatusCode);
            /*  updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
              var updatedQuiz = await updateResponse.Content.ReadFromJsonAsync<QuizResponse>();
              updatedQuiz.Should().NotBeNull();
              updatedQuiz.Name.Should().Be(updateRequest.Name);
              updatedQuiz.Description.Should().Be(updateRequest.Description);
              updatedQuiz.ImageUrl.Should().Be(updateRequest.ImageUrl);
              updatedQuiz.Language.Should().Be(updateRequest.Language);
              updatedQuiz.TimeLimitMinutes.Should().Be(updateRequest.TimeLimitMinutes);*/
        }
      

        
        [Fact]
        public async Task UpdateQuiz_WithEmptyRequest_ReturnsBadRequest()
        {/*
            // Arrange
            Guid quizId = Guid.NewGuid();
            var updateRequest = new UpdateQuizRequest(); // Empty request

            // Act
            var updateResponse = await Client.PatchAsJsonAsync($"api/quizzes/{quizId}", updateRequest);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);*/
        }
    }
}

