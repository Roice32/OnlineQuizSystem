
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.WebSocket;
using OQS.CoreWebAPI.Tests.SetUp;
using Xunit;

namespace QQS.CoreWebAPI.Tests
{
    public class JoinLiveQuizTests : ApplicationContextForTesting
    {
        private readonly ApplicationDBContext _context;

        public JoinLiveQuizTests(ApplicationDBContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task JoinRoomTest()
        {
            // Arrange
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Seeder>());
            var hubContext = server.Host.Services.GetService<IHubContext<QuizRoomHub>>();
            var client = new HubConnectionBuilder()
                .WithUrl("http://localhost/quizroomhub", options =>
                {
                    options.HttpMessageHandlerFactory = _ => server.CreateHandler();
                })
                .Build();

            await client.StartAsync();

            var connectionId = Guid.NewGuid();

            // Act
            await client.InvokeAsync("JoinRoom", new UserConnection
            {
                UserId = Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d"),
                ConnectionId = connectionId.ToString()
            });

            // Assert
            var userConnection = await _context.UserConnections.FindAsync(connectionId);
            Assert.NotNull(userConnection); // Verifies that the UserConnection was saved in the database
            Assert.Equal(Guid.Parse("5b048913-5df0-429f-a42b-051904672e4d"), userConnection.UserId); // Verifies that the UserId is correct
        }
    }
}