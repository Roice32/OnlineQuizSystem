using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;
using ConnectionRequest = OQS.CoreWebAPI.Contracts.LiveQuizzes.ConnectionRequest;

namespace QQS.CoreWebAPI.Tests
{
    public class JoinLiveQuizTests : ApplicationContextForTesting
    {
      
           [Fact]
        public async Task JoinRoomTest() 
        {
            // Arrange
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddSignalR();
                    });
                });

            var client = factory.CreateClient();
            var connection = new HubConnectionBuilder()
                .WithUrl(client.BaseAddress.AbsoluteUri + "/ws/live-quizzes")
                .Build();

            var userJoinedReceived = new TaskCompletionSource<bool>();
            connection.On("ReceiveMessage", (string message) =>
            {
                if (message.Contains("has joined the room"))
                {
                    userJoinedReceived.SetResult(true);
                }
            });

            await connection.StartAsync();

            var connectionRequest = new ConnectionRequest()
            {
                UserId = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d").ToString(), // Use a random user ID
                Code = "pv00SE" // Use a test code
            };

            // Act
            await connection.InvokeAsync("JoinRoom", connectionRequest);

            // Assert
            var userJoined = await Task.WhenAny(userJoinedReceived.Task, Task.Delay(5000)); // Wait for up to 5 seconds
            Assert.True(userJoined == userJoinedReceived.Task, "UserJoined was not received");
        }
    }
}