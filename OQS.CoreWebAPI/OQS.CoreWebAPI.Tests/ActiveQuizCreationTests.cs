using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace OQS.CoreWebAPI.Tests;

public class ActiveQuizCreationTests: ApplicationContextForTesting
{
   

    [Fact]
    public async Task CreateActiveQuiz_ReturnsOkStatus()
    {


        var newQuiz = new CreateActiveQuizRequest()
        {
            QuizId = Guid.Parse("00000000-0000-0000-0002-000000000001"),
        };
        var response= await Client.PostAsJsonAsync("api/active-quizzes", newQuiz);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateActiveQuiz_Invalid_QuizId()
    {    
        var newQuiz = new CreateActiveQuizRequest()
        {
            QuizId = Guid.Parse("00000000-0000-0000-0002-000000000010"),
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
            QuizId = Guid.Parse("00000000-0000-0000-0002-000000000001"),
            
        };
    
        Client.DefaultRequestHeaders.Remove("Authorization");
        Client.DefaultRequestHeaders.Add("Authorization", "Bearer  eyJhbGciOi");
        var response = await Client.PostAsJsonAsync("api/active-quizzes", newQuiz);
    
    
        var result = await response.Content.ReadFromJsonAsync<Result<Guid>>();
        Assert.True(result.IsFailure);
        Assert.Equal("CreateActiveQuiz.BadRequest", result.Error.Code); 
        Assert.Equal("Invalid Token", result.Error.Message);
    }
}

