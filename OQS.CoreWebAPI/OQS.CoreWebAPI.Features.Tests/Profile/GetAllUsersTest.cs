using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Profile;
using Xunit;

public class GetAllUsersTests
{
    /*[Fact]
    public async Task Handle_ValidJwt_ReturnsListOfUsers()
    {
        // Arrange
        var userManager = Substitute.For<UserManager<User>>(
            Substitute.For<IUserStore<User>>(),
            null, null, null, null, null, null, null, null
        );
        userManager.Users.Returns(new List<User>
        {
            new User { UserName = "user1" },
            new User { UserName = "user2" }
        }.AsQueryable());

        var configuration = Substitute.For<IConfiguration>();
        configuration["JWT:Secret"].Returns("test_secret_12345678901234567890123456789012");
        configuration["JWT:ValidIssuer"].Returns("valid_issuer");
        configuration["JWT:ValidAudience"].Returns("valid_audience");

        var handler = new GetAllUsers.Handler(userManager, configuration);
        var query = new GetAllUsers.Query { Jwt = "valid_jwt" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task Handle_InvalidJwt_ReturnsFailure()
    {
        // Arrange
        var userManager = Substitute.For<UserManager<User>>(
            Substitute.For<IUserStore<User>>(),
            null, null, null, null, null, null, null, null
        );

        var configuration = Substitute.For<IConfiguration>();
        configuration["JWT:Secret"].Returns("test_secret_12345678901234567890123456789012");
        configuration["JWT:ValidIssuer"].Returns("valid_issuer");
        configuration["JWT:ValidAudience"].Returns("valid_audience");

        var handler = new GetAllUsers.Handler(userManager, configuration);
        var query = new GetAllUsers.Query { Jwt = "invalid_jwt" };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid Jwt", result.Error.Message);
    }*/
}
