using Xunit;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Profile;
using OQS.CoreWebAPI.Contracts.Models;
using Microsoft.Extensions.Configuration;

namespace OQS.CoreWebAPI.Tests.Profile
{
    public class UpdateUserTest
    {
        [Fact]
        public async Task Handle_ValidUpdate_ReturnsSuccess()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var configurationMock = Substitute.For<IConfiguration>();

            var command = new UpdateUser.Command
            {
                Id = Guid.NewGuid(),
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Username = "UpdatedUsername",
                Email = "updated@example.com",
                Jwt = "valid_jwt_token"
            };

            // Simulate an existing user
            var user = new User
            {
                Id = command.Id.ToString(),
                UserName = "OldUsername",
                FirstName = "OldFirstName",
                LastName = "OldLastName",
                Email = "old@example.com"
            };
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns(user);
            userManagerMock.UpdateAsync(user).Returns(IdentityResult.Success);

            var handler = new UpdateUser.Handler(userManagerMock, configurationMock);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_InvalidJwt_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var configurationMock = Substitute.For<IConfiguration>();

            var command = new UpdateUser.Command
            {
                Id = Guid.NewGuid(),
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Username = "UpdatedUsername",
                Email = "updated@example.com",
                Jwt = "invalid_jwt_token"
            };

            var handler = new UpdateUser.Handler(userManagerMock, configurationMock);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var configurationMock = Substitute.For<IConfiguration>();

            var command = new UpdateUser.Command
            {
                Id = Guid.NewGuid(),
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Username = "UpdatedUsername",
                Email = "updated@example.com",
                Jwt = "valid_jwt_token"
            };

            // Simulate a non-existing user
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns((User)null);

            var handler = new UpdateUser.Handler(userManagerMock, configurationMock);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }

        [Fact]
        public async Task Handle_UpdateFailed_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var configurationMock = Substitute.For<IConfiguration>();

            var command = new UpdateUser.Command
            {
                Id = Guid.NewGuid(),
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Username = "UpdatedUsername",
                Email = "updated@example.com",
                Jwt = "valid_jwt_token"
            };

            // Simulate an existing user
            var user = new User
            {
                Id = command.Id.ToString(),
                UserName = "OldUsername",
                FirstName = "OldFirstName",
                LastName = "OldLastName",
                Email = "old@example.com"
            };
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns(user);
            userManagerMock.UpdateAsync(user).Returns(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            var handler = new UpdateUser.Handler(userManagerMock, configurationMock);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }
    }
}
