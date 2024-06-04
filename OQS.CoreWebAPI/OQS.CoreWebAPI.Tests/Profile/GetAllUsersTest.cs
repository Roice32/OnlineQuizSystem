using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Xunit;
using OQS.CoreWebAPI.Features.Profile;
using OQS.CoreWebAPI.Shared;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace OQS.CoreWebAPI.Features.Profile.Tests
{
    public class GetAllUsersTest
    {
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






       /* [Fact]
        public async Task Handle_InvalidJwt_ReturnsAuthenticationError()
        {
            // Arrange
            var configurationMock = CreateMockConfiguration();
            var userManagerMock = Substitute.For<UserManager<User>>();
            var invalidJwt = "invalid_jwt_token";

            var handler = new GetAllUsers.Handler(userManagerMock, configurationMock);

            var query = new GetAllUsers.Query
            {
                Jwt = invalidJwt
            };

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }


        [Fact]
        public async Task Handle_ValidNonAdminJwt_ReturnsAuthorizationError()
        {
            // Arrange
            var configurationMock = CreateMockConfiguration();
            var store = Substitute.For<IUserStore<User>>();
            var userManagerMock = Substitute.ForPartsOf<UserManager<User>>(store, null, null, null, null, null, null, null, null);
            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012", "regularUser", "User");

            var handler = new GetAllUsers.Handler(userManagerMock, configurationMock);

            var query = new GetAllUsers.Query
            {
                Jwt = jwt
            };

            // Act
            var result = await handler.Handle(query, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("You are not an admin.", result.Error.Message);
        }

    */

    }
}
