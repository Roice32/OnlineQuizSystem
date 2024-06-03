using System.Net.Http.Json;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace OQS.CoreWebAPI.Tests
{
    public class GetActiveQuizByIdTest : ApplicationContextForTesting
    {
        [Fact]
        public async Task GetActiveQuizById_ReturnsOk()
        {
            
            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450");
            
            var response = await Client.GetAsync($"api/active-quizzes/{activeQuizId}");
            
            
            var result = await response.Content.ReadFromJsonAsync<Result<String>>();
            
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public async Task GetActiveQuizById_Invalid_ActiveQuizId()
        {
            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972453");
            
            var response = await Client.GetAsync($"api/active-quizzes/{activeQuizId}");
            
            
            var result = await response.Content.ReadFromJsonAsync<Result<String>>();
            
            Assert.NotNull(result);
            Assert.True(result.IsFailure);
            Assert.Equal("GetActiveQuiz.BadRequest", result.Error.Code); 
            Assert.Equal("Invalid Quiz Id\r\nUser is not associated with the given active quiz ID", result.Error.Message);
        }
        
        [Fact]
        public async Task GetActiveQuizById_Invalid_Token()
        {
            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450");
            Client.DefaultRequestHeaders.Remove("Authorization");
            Client.DefaultRequestHeaders.Add("Authorization", "Bearer  eyJhbGciOi");
            
            var response = await Client.GetAsync($"api/active-quizzes/{activeQuizId}");
            
            var result = await response.Content.ReadFromJsonAsync<Result<String>>();
            
            Assert.NotNull(result);
            Assert.True(result.IsFailure);
            Assert.Equal("GetActiveQuiz.BadRequest", result.Error.Code); 
            Assert.Equal("Invalid Token\r\nUser is not associated with the given active quiz ID", result.Error.Message);
        }
        
    }
}