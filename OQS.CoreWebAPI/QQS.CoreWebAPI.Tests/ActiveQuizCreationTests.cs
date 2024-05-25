using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace QQS.CoreWebAPI.Tests;

public class ActiveQuizCreationTests: ApplicationContextForTesting
{
   

    [Fact]
    public async Task CreateActiveQuiz_ReturnsOkStatus()
    {


        var newQuiz = new CreateActiveQuizRequest()
        {
            QuizId = Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"),
            TakenBy = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d").ToString()
        };

        
        var response = await Client.PostAsJsonAsync("api/active-quizzes", newQuiz);

      

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateActiveQuiz_Invalid_QuizId()
    {    
        var newQuiz = new CreateActiveQuizRequest()
        {
            QuizId = Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13a"),
            TakenBy = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d").ToString()
        };
    
        
        var response = await Client.PostAsJsonAsync("api/active-quizzes", newQuiz);
        
        var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
        Assert.True(result.IsFailure);
        Assert.Equal("CreateActiveQuiz.BadRequest", result.Error.Code); 
        Assert.Equal("Invalid Quiz Id", result.Error.Message);
    }

    [Fact]
    public async Task CreateActiveQuiz_Invalid_UserId()
    {
        var newQuiz = new CreateActiveQuizRequest()
        {
            QuizId = Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"),
            TakenBy = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4a").ToString()
        };


        var response = await Client.PostAsJsonAsync("api/active-quizzes", newQuiz);

   
        var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
        Assert.True(result.IsFailure);
        Assert.Equal("CreateActiveQuiz.BadRequest", result.Error.Code); 
        Assert.Equal("Invalid User Id", result.Error.Message);
    }
}

