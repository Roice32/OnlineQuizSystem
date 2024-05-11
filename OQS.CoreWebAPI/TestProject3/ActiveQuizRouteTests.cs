using System.Net;
using System.Net.Http.Json;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Features;
using OQS.CoreWebAPI.Shared;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OQS.CoreWebAPI.Database;
using TestActiveQuiz.SetUp;

// using Microsoft.AspNetCore.TestHost;

namespace TestCreateActiveQuiz
{
    public class ActiveQuizCreationTests : ActiveQuizApplicationContextForTesting
    {

        //crearea unui active quiz și întoarcerea unui răspuns HTTP de succes.
        [Fact]
        public async Task CreateActiveQuiz_ReturnsCreatedStatus()
        {
            // Arrange
            using var scope = Application.Services.CreateScope();
        
            var newQuiz = new
            {
                QuizId = Guid.NewGuid(), // Un identificator nou pentru quiz
                TakenBy = Guid.NewGuid() // Un identificator nou pentru userul care a luat quiz-ul
            };

            // Act
            var response = await Client.PostAsJsonAsync("api/active-quiz", newQuiz);

            // Assert
            response.EnsureSuccessStatusCode(); // Verifică dacă răspunsul HTTP indică o operație de succes (codul 2xx)
            Assert.Equal(HttpStatusCode.Created, response.StatusCode); // Verifică dacă răspunsul HTTP este unul de tip Created (codul 201)
        }
    }
}   