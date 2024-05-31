using FluentAssertions;
using Moq;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Features.LiveQuizzes;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Shared;
using Xunit;

namespace OQS.CoreWebAPI.Tests
{
   public class CancelLiveQuizTests
    {
        [Fact]
        public async Task CancelLiveQuiz_Admin_Success()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new ApplicationDBContext(options))
            {
                var adminUser = new User { Id = Guid.NewGuid().ToString(), FirstName = "Admin", Email = "admin@example.com"};
                var liveQuiz = new LiveQuizz
                {
                    Code = "ABC123",
                    CreatedBy = adminUser,
                    Connections = new List<UserConnection>
                    {
                        new UserConnection { ConnectionId = "conn123", User = adminUser }
                    }
                };

                dbContext.LiveQuizzes.Add(liveQuiz);
                await dbContext.SaveChangesAsync();

                var validator = new Mock<IValidator<CancelLiveQuiz.Command>>();
                validator.Setup(v => v.ValidateAsync(It.IsAny<CancelLiveQuiz.Command>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult());

                var mockClients = new Mock<IHubClients>();
                var mockClientProxy = new Mock<IClientProxy>();

                mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

                var hubContext = new Mock<IHubContext<LiveQuizzesHub>>();
                hubContext.Setup(hub => hub.Clients).Returns(mockClients.Object);

                var handler = new CancelLiveQuiz.Handler(dbContext, hubContext.Object, validator.Object);

                // Act
                var result = await handler.Handle(new CancelLiveQuiz.Command("conn123"), CancellationToken.None);

                // Assert
                result.Should().BeEquivalentTo(Result.Success());
            }
        }

        [Fact]
        public async Task CancelLiveQuiz_NotAdmin_Failure()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new ApplicationDBContext(options))
            {
                var adminUser = new User { Id = Guid.NewGuid().ToString(), FirstName = "Admin", Email = "admin@example.com"};
                var nonAdminUser = new User { Id = Guid.NewGuid().ToString(), FirstName = "Non Admin", Email = "nonadmin@example.com" };

                var liveQuiz = new LiveQuizz
                {
                    Code = "ABC123",
                    CreatedBy = adminUser,
                    Connections = new List<UserConnection>
                    {
                        new UserConnection { ConnectionId = "adminConn", User = adminUser },
                        new UserConnection { ConnectionId = "nonAdminConn", User = nonAdminUser }
                    }
                };

                dbContext.LiveQuizzes.Add(liveQuiz);
                await dbContext.SaveChangesAsync();

                var validator = new Mock<IValidator<CancelLiveQuiz.Command>>();
                validator.Setup(v => v.ValidateAsync(It.IsAny<CancelLiveQuiz.Command>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new FluentValidation.Results.ValidationResult());

                var mockClients = new Mock<IHubClients>();
                var mockClientProxy = new Mock<IClientProxy>();

                mockClients.Setup(clients => clients.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

                var hubContext = new Mock<IHubContext<LiveQuizzesHub>>();
                hubContext.Setup(hub => hub.Clients).Returns(mockClients.Object);

                var handler = new CancelLiveQuiz.Handler(dbContext, hubContext.Object, validator.Object);

                // Act
                var result = await handler.Handle(new CancelLiveQuiz.Command("nonAdminConn"), CancellationToken.None);

                // Assert
                result.Should().BeEquivalentTo(Result.Failure(new Error("CancelLiveQuiz.Unauthorized", "User is not the admin")));
            }
        }
    }
}