using FluentAssertions;
using Microsoft.AspNetCore.Http;
using OQS.CoreWebAPI.Tests.SetUp;
using System.Net;
using Xunit;

namespace QQS.CoreWebAPI.Tests
{
    public class QuizDeletionTests : ApplicationContextForTesting
        {
        [Fact]
        public async Task DeleteQuiz_ReturnsOkStatus()
        {

            // Arrange
            HttpRequest request;
           Guid quizId = Guid.Parse("00000000-0000-1000-0004-000000000000");

           // Act
           var deleteResponse = await Client.DeleteAsync($"api/quizzes/{quizId}");

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK); // Verifică dacă ștergerea a avut succes 
        }


        [Fact]
            public async Task DeleteQuiz_WithInvalidId_ReturnsBadRequest()
            {
                // Arrange
                var invalidId = Guid.Empty;

                // Act
                var deleteResponse = await Client.DeleteAsync($"api/quizzes/{invalidId}");

                // Assert
                deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                var responseContent = await deleteResponse.Content.ReadAsStringAsync();
                responseContent.Should().Contain("Id is required.");
            }

            [Fact]
            public async Task DeleteQuiz_WithNonExistentId_ReturnsNotFound()
            {
                // Arrange
                var nonExistentId = Guid.NewGuid();

                // Act
                var deleteResponse = await Client.DeleteAsync($"api/quizzes/{nonExistentId}");

                // Assert
                deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
                var responseContent = await deleteResponse.Content.ReadAsStringAsync();
                responseContent.Should().Contain("Quiz not found.");
            }
        }
    }

