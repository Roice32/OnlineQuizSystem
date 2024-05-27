using Xunit;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Profile;
using OQS.CoreWebAPI.Contracts.Models;

namespace OQS.CoreWebAPI.Tests.Profile
{
    public class GetUserDetailsTest
    {
        /*[Fact]
        public async Task Handle_ValidUserId_ReturnsUserDetails()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var query = new Feautures.Profile.GetUserDetails.Query
            {
                Id = Guid.NewGuid(),
                Jwt = "valid_jwt_token"
            };

            // Simulate an existing user
            var user = new User
            {
                Id = query.Id.ToString(),
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com"
            };
            userManagerMock.FindByIdAsync(query.Id.ToString()).Returns(user);

            var handler = new Feautures.Profile.GetUserDetails.Handler(userManagerMock, null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("testuser", result.Value.Username);
            Assert.Equal("Test", result.Value.FirstName);
            Assert.Equal("User", result.Value.LastName);
            Assert.Equal("testuser@example.com", result.Value.Email);
        }
      */
        [Fact]
        public async Task Handle_InvalidUserId_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var query = new Feautures.Profile.GetUserDetails.Query
            {
                Id = Guid.NewGuid(),
                Jwt = "valid_jwt_token"
            };

            // Simulate a non-existing user
            userManagerMock.FindByIdAsync(query.Id.ToString()).Returns((User)null);

            var handler = new Feautures.Profile.GetUserDetails.Handler(userManagerMock, null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }

        [Fact]
        public async Task Handle_InvalidJwt_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var query = new Feautures.Profile.GetUserDetails.Query
            {
                Id = Guid.NewGuid(),
                Jwt = "invalid_jwt_token"
            };

            var handler = new Feautures.Profile.GetUserDetails.Handler(userManagerMock, null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }
    }
}
