using Xunit;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Profile;
using OQS.CoreWebAPI.Contracts.Models;
using Org.BouncyCastle.Bcpg.Sig;
using Microsoft.Extensions.Configuration;
using OQS.CoreWebAPI.Features.Authentication;
using System.ComponentModel.DataAnnotations;

namespace OQS.CoreWebAPI.Features.Tests.Profile
{
    public class GetUserDetailsTest
    {
       /* [Fact]
        public async Task Handle_ValidRequest_ReturnsUserDetails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var jwtToken = "valid_jwt_token";

            // Simulează un UserManager care returnează un utilizator existent
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var expectedUser = new User
            {
                Id = userId.ToString(),
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com"
            };
            userManagerMock.FindByIdAsync(userId.ToString()).Returns(expectedUser);

  
            var configurationMock = Substitute.For<IConfiguration>();
            configurationMock["JWT:Secret"].Returns("test_secret_12345678901234567890123456789012");
            configurationMock["JWT:ValidIssuer"].Returns("valid_issuer");
            configurationMock["JWT:ValidAudience"].Returns("valid_audience");

          
            var jwtValidatorMock = Substitute.For<JwtValidator>(configurationMock);
            jwtValidatorMock.Validate(Arg.Any<string>()).Returns(true);

            var handler = new Features.Profile.GetUserDetails.Handler(userManagerMock, configurationMock);
            var query = new Features.Profile.GetUserDetails.Query
            {
                Id = userId,
                Jwt = jwtToken
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(expectedUser.UserName, result.Value.Username);
            Assert.Equal(expectedUser.FirstName, result.Value.FirstName);
            Assert.Equal(expectedUser.LastName, result.Value.LastName);
            Assert.Equal(expectedUser.Email, result.Value.Email);
        }*/

        [Fact]
        public async Task Handle_InvalidJwt_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var query = new Features.Profile.GetUserDetails.Query
            {
                Id = Guid.NewGuid(),
                Jwt = "invalid_jwt_token"
            };

            var handler = new Features.Profile.GetUserDetails.Handler(userManagerMock, null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }
    }
}
