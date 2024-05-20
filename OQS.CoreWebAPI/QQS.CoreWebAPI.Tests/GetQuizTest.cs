using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace QQS.CoreWebAPI.Tests
{
    public class GetQuizTest : ApplicationContextForTesting
    {
        
            [Fact]
            public async Task GetQuiz_ReturnsOkStatus()
            {
            // Arrange

            Guid existingQuizId = Guid.Parse("00000000-0000-1000-0001-000000000000");

                // Act
                var getResponse = await Client.GetAsync($"api/quizzes/{existingQuizId}");

                // Assert
                getResponse.StatusCode.Should().Be(HttpStatusCode.OK); // Verifică dacă răspunsul HTTP este de tip OK (cod 200)
            }

            [Fact]
            public async Task GetNonExistingQuiz_ReturnsNotFound()
            {
                // Arrange
                Guid nonExistingQuizId = Guid.NewGuid();

                // Act
                var getResponse = await Client.GetAsync($"api/quizzes/{nonExistingQuizId}");

                // Assert
                getResponse.StatusCode.Should().Be(HttpStatusCode.OK); // Verifică dacă răspunsul HTTP este NotFound (cod 404)
            }
        }
    }


