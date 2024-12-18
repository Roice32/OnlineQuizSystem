using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace OQS.CoreWebAPI.Tests;

public class CreateLiveQuizTest : ApplicationContextForTesting
{
    
    [Fact]
    public async Task CreateLiveQuiz_ReturnsOk()
    {
        var newLiveQuiz = new CreateLiveQuizRequest()
        {
            QuizId = Guid.Parse("00000000-0000-0000-0002-000000000001"),
        };
        var response = await Client.PostAsJsonAsync("api/live-quizzes", newLiveQuiz);
      

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Result<String>>();
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateLiveQuiz_Invalid_QuizId()
    {    
        var newLiveQuiz = new CreateLiveQuizRequest()
        {
            QuizId = Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13a"),
        };
        var response = await Client.PostAsJsonAsync("api/live-quizzes", newLiveQuiz);
      
        
        var result = await response.Content.ReadFromJsonAsync<Result<String>>();
        Assert.NotNull(result);
        Assert.Equal("CreateLiveQuiz.BadRequest", result.Error.Code); 
        Assert.Equal("Invalid Quiz Id", result.Error.Message);
    }

    [Fact]
    public async Task CreateActiveQuiz_Invalid_Token()
    {
        var newLiveQuiz = new CreateLiveQuizRequest()
        {
            QuizId = Guid.Parse("00000000-0000-0000-0002-000000000001"),
        };
        
        Client.DefaultRequestHeaders.Remove("Authorization");
        Client.DefaultRequestHeaders.Add("Authorization", "Bearer  eyJhbGciOi");
        
        
        var response = await Client.PostAsJsonAsync("api/live-quizzes", newLiveQuiz);
        var result = await response.Content.ReadFromJsonAsync<Result<String>>();
        
        
        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("CreateLiveQuiz.BadRequest", result.Error.Code); 
        Assert.Equal("Invalid Token", result.Error.Message);
    }
}