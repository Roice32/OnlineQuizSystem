
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.LiveQuizzes;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;
using ConnectionRequest = OQS.CoreWebAPI.Contracts.LiveQuizzes.ConnectionRequest;

namespace QQS.CoreWebAPI.Tests
{
    public class JoinLiveQuizTests : IClassFixture<WebApplicationFactory<ApplicationContextForTesting>>
    {
        private readonly WebApplicationFactory<ApplicationContextForTesting> _factory;
        private readonly ApplicationDBContext _context;

        public JoinLiveQuizTests(WebApplicationFactory<ApplicationContextForTesting> factory)
        {
            _factory = factory;
            _context = factory.Services.GetService<ApplicationDBContext>();
        }

        [Fact]
        public async Task JoinRoomTest()
        {
            // Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IHubContext<LiveQuizzesHub>>();
                });
            }).CreateClient();

            var connection = new HubConnectionBuilder()
                .WithUrl(client.BaseAddress.AbsoluteUri + "/ws/live-quizzes")
                .Build();

            await connection.StartAsync();

            var connectionId = Guid.NewGuid();
            // Ensure the user and live quiz exist in the database
            var userId = _context.Users.First().Id; // Use an existing user ID
            var liveQuizCode = _context.LiveQuizzes.First().Code; // Use an existing live quiz code

            
            // Act
            await connection.InvokeAsync("JoinRoom", new ConnectionRequest()
            {
                // UserId = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d"),
                // ConnectionId = connectionId.ToString()
                UserId = userId,
                Code = liveQuizCode
            });

            // Assert
            var userConnection = await _context.UserConnections.FindAsync(connectionId);
            Assert.NotNull(userConnection); // Verifies that the UserConnection was saved in the database
            Assert.Equal(userId, userConnection.UserId); // Verifies that the UserId is correct
        }
    }
}