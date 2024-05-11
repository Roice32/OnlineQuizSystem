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
//     public class IntegrationTests
// {
//     private readonly TestServer _server;
//     private readonly HttpClient _client;
//
//     public IntegrationTests()
//     {
//         // Creează un server de test și un client HTTP
//         _server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
//         _client = _server.CreateClient();
//     }
//
//     [Fact]
//     public async Task Post_ActiveQuiz_ReturnsSuccessResult_WhenQuizCreationIsValid()
//     {
//         // Arrange
//         var quizId = Guid.NewGuid();
//         var takenBy = Guid.NewGuid();
//         var request = new CreateActiveQuizRequest { QuizId = quizId, TakenBy = takenBy };
//
//         // Act
//         var response = await _client.PostAsync("api/active-quiz", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
//
//         // Assert
//         response.EnsureSuccessStatusCode();
//
//         var responseString = await response.Content.ReadAsStringAsync();
//         var result = JsonConvert.DeserializeObject<Result<Guid>>(responseString);
//
//         Assert.True(result.IsSuccess);
//     }
// }

    // public class ActiveQuizRouteTests
    //  {
    //      [Fact]
    //      public async Task MapPost_ReturnsSuccessResult_WhenQuizCreationIsValid()
    //      {
    //          // Arrange
    //          var quizId = Guid.NewGuid();
    //          var takenBy = Guid.NewGuid();
    //          var request = new CreateActiveQuizRequest { QuizId = quizId, TakenBy = takenBy };
    //          var quizCreation = new CreateActiveQuiz.QuizCreation(request.QuizId, request.TakenBy);
    //
    //          var senderMock = new Mock<ISender>();
    //          senderMock.Setup( s => s.Send(It.Is<CreateActiveQuiz.QuizCreation>(q => q.QuizId == quizId && q.TakenBy == takenBy)))
    //              .ReturnsAsync(Result.Success(new ActiveQuiz { Id = Guid.NewGuid() }));
    //
    //          // Act
    //          var endpointFunction = new CreateActiveQuiz.CreateActiveQuizEndPoint().MapPostFunction;
    //          var result = await endpointFunction(request, senderMock.Object);
    //
    //          // Assert
    //          Assert.IsType<Result<Guid>>(result);
    //          Assert.True((bool)result.IsSuccess);
    //      }
    //
    // }

    public class ActiveQuizCreationTests : ActiveQuizApplicationContextForTesting
    {
        // private readonly HttpClient _client;
        //
        // public ActiveQuizCreationTests()
        // {
        //     _client = Client;
        // }

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