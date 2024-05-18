using System.Net;
using System.Net.Http.Json;
using OQS.CoreWebAPI.Contracts;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace QQS.CoreWebAPI.Tests
{
    public class GetActiveQuizByIdTest : ApplicationContextForTesting
    {
        [Fact]
        public async Task GetActiveQuizById_ReturnsOk()
        {
            var client = Application.CreateClient();
            
            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450");
            String userId = "5b048913-5df0-429f-a42b-051904672e4d";
            
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/active-quizzes/{activeQuizId}");
            request.Headers.Add("Cookie", $"userId={userId}");
            
            var response = await client.SendAsync(request);
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<Result<ActiveQuizResponse>>();
            
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }
        
        [Fact]
        public async Task GetActiveQuizById_Invalid_ActiveQuizId()
        {
            var client = Application.CreateClient();
            
            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972453");
            String userId = "5b048913-5df0-429f-a42b-051904672e4d";
            
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/active-quizzes/{activeQuizId}");
            request.Headers.Add("Cookie", $"userId={userId}");
            
            var response = await client.SendAsync(request);
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<Result<ActiveQuizResponse>>();
            
            Assert.NotNull(result);
            Assert.True(result.IsFailure);
            Assert.Equal("GetActiveQuiz.BadRequest", result.Error.Code); 
            Assert.Equal("Invalid Active Quiz Id\r\nUser is not associated with the given active quiz ID", result.Error.Message);
        }
        
        [Fact]
        public async Task GetActiveQuizById_Invalid_UserId()
        {
            var client = Application.CreateClient();
            
            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450");
            String userId = "5b048913-5df0-429f-a42b-051904672e4f";
            
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/active-quizzes/{activeQuizId}");
            request.Headers.Add("Cookie", $"userId={userId}");
            
            var response = await client.SendAsync(request);
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<Result<ActiveQuizResponse>>();
            
            Assert.NotNull(result);
            Assert.True(result.IsFailure);
            Assert.Equal("GetActiveQuiz.BadRequest", result.Error.Code); 
            Assert.Equal("Invalid User ID\r\nUser is not associated with the given active quiz ID", result.Error.Message);
        }
        
        [Fact]
        public async Task GetActiveQuizById_UserId_Not_Found_In_Cookie()
        {
            var client = Application.CreateClient();
            
            var activeQuizId = Guid.Parse("f0a486df-a7bd-467f-bb9a-4ac656972450");
            
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/active-quizzes/{activeQuizId}");
            
            var response = await client.SendAsync(request);
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<Result<ActiveQuizResponse>>();
            
            Assert.NotNull(result);
            Assert.True(result.IsFailure);
            Assert.Equal("GetActiveQuiz.BadRequest", result.Error.Code); 
            Assert.Equal("User ID not found in the cookie", result.Error.Message);
        }
        
    }
}