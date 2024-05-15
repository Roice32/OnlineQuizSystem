using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace QQS.CoreWebAPI.Tests;

public class ActiveQuizCreationTests : ApplicationContextForTesting
{
    [Fact]
    public async Task CreateActiveQuiz_ReturnsOkStatus()
    {
        // var newQuiz = new CreateActiveQuizRequest()
        // {
        //     quizId = Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"),
        //     takenBy = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d")
        // };

        // 
        // var response = await Client.PostAsJsonAsync("api/active-quiz", newQuiz);

        // // Assert
        // response.EnsureSuccessStatusCode(); // Verifică dacă răspunsul HTTP indică o operație de succes (codul 2xx)
        // Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Verifică dacă răspunsul HTTP este unul de tip Created (codul 201)
    }
}