using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Profile;
using OQS.CoreWebAPI.Shared;
using Xunit;

namespace QOS.CoreWebAPI.Feautures.Tests.Profile
{
    public class GetUserDetailsTest
    {
        private UserManager<User> CreateMockUserManager()
        {
            var userStoreMock = Substitute.For<IUserStore<User>>();
            return Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);
        }

        private IConfiguration CreateMockConfiguration()
        {
            var configurationMock = Substitute.For<IConfiguration>();
            configurationMock["JWT:Secret"].Returns("test_secret_12345678901234567890123456789012");
            configurationMock["JWT:ValidIssuer"].Returns("valid_issuer");
            configurationMock["JWT:ValidAudience"].Returns("valid_audience");
            return configurationMock;
        }

        private string GenerateJwtToken(string issuer, string audience, string secret, string uniqueName, string role)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new System.Security.Claims.Claim("unique_name", uniqueName),
                new System.Security.Claims.Claim("role", role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /*

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();
            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012", "testUser", "Admin");

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId.ToString(),
                UserName = "testUser",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com"
            };

            userManagerMock.FindByIdAsync(userId.ToString())
                .Returns(Task.FromResult(user));

            var handler = new GetUserDetails.Handler(userManagerMock, configurationMock);

            var query = new GetUserDetails.Query
            {
                Id = userId,
                Jwt = jwt
            };

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("testUser", result.Value.Username);
            Assert.Equal("Test", result.Value.FirstName);
            Assert.Equal("User", result.Value.LastName);
            Assert.Equal("testuser@example.com", result.Value.Email);
        }

        [Fact]
        public async Task Handle_InvalidJwt_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();
            var invalidJwt = "invalid_jwt_token";

            var handler = new GetUserDetails.Handler(userManagerMock, configurationMock);

            var query = new GetUserDetails.Query
            {
                Id = Guid.NewGuid(),
                Jwt = invalidJwt
            };

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();
            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012", "testUser", "Admin");

            var userId = Guid.NewGuid();
            userManagerMock.FindByIdAsync(userId.ToString())
                .Returns(Task.FromResult<User>(null));

            var handler = new GetUserDetails.Handler(userManagerMock, configurationMock);

            var query = new GetUserDetails.Query
            {
                Id = userId,
                Jwt = jwt
            };

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User doesn't exists.", result.Error.Message);
        }

        */
    }
}
