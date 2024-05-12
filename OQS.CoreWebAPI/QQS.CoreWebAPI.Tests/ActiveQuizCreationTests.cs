using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Features;
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
            TakenBy = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d")
        };

        
        var response = await Client.PostAsJsonAsync("api/active-quiz", newQuiz);

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        // response.EnsureSuccessStatusCode(); // Verifică dacă răspunsul HTTP indică o operație de succes (codul 2xx)
        // Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Verifică dacă răspunsul HTTP este unul de tip Created (codul 201)
        //
        // var result = await response.Content.ReadFromJsonAsync<Result<ActiveQuiz>>();
        // Assert.IsType<Result<ActiveQuiz>>(result); // Verifică dacă rezultatul este de tipul Result<ActiveQuiz>

    }

    // [Fact]
    // public async Task CreateActiveQuiz_Invalid_UserId()
    // {    
    //     var newQuiz = new CreateActiveQuizRequest()
    //     {
    //         QuizId = Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13a"),
    //         TakenBy = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d")
    //     };
    //
    //     
    //     var response = await Client.PostAsJsonAsync("api/active-quiz", newQuiz);
    //     
    //     // Assert
    //     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // Verifică dacă răspunsul HTTP este de tip 400
    //
    // }
    // [Fact]
    // public async Task CreateActiveQuiz_Invalid_QuizId()
    // {
    //     var newQuiz = new CreateActiveQuizRequest()
    //     {
    //         QuizId = Guid.Parse("1af3912f-d625-413a-91b6-cb31f4cbb13b"),
    //         TakenBy = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4a")
    //     };
    //
    //     
    //     var response = await Client.PostAsJsonAsync("api/active-quiz", newQuiz);
    //     
    //     // Assert
    //   
    //     Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); // Verifică dacă răspunsul HTTP este de tip 400
    //
    // }
}

public class QuizCreationValidatorTests
{
    private CreateActiveQuiz.QuizCreationValidator _validator;
    private Mock<IServiceScopeFactory> _serviceScopeFactory;

    public QuizCreationValidatorTests()
    {
        _serviceScopeFactory = new Mock<IServiceScopeFactory>();
        _validator = new CreateActiveQuiz.QuizCreationValidator(_serviceScopeFactory.Object);
    }

    [Fact]
    public async Task Validate_ShouldReturnFalse_WhenQuizIdDoesNotExist()
    {
        // Arrange
        var quizCreation = new CreateActiveQuiz.QuizCreation(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var validationResult = await _validator.ValidateAsync(quizCreation);

        // Assert
        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, x => x.ErrorMessage == "Invalid Quiz Id");
    }


    // Similar tests for other validation rules...
}