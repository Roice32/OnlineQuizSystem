using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using QOS.CoreWebAPI.Features.Profile;
using Xunit;

namespace QOS.CoreWebAPI.Feautures.Profile.Tests
{
    public class GetUserIdTest
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

        [Fact]
        public async Task Handle_ValidJwt_ReturnsSuccess()
        {
            // Arrange
            var configurationMock = CreateMockConfiguration();
            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012", "testUser", "Admin");

            var handler = new GetUserId.Handler(configurationMock);

            var command = new GetUserId.Command
            {
                Jwt = jwt
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("testUser", result.Value.Id);
            Assert.Equal("Admin", result.Value.Role);
        }

        [Fact]
        public async Task Handle_InvalidJwt_ReturnsFailure()
        {
            // Arrange
            var configurationMock = CreateMockConfiguration();
            var invalidJwt = "invalid_jwt_token";

            var handler = new GetUserId.Handler(configurationMock);

            var command = new GetUserId.Command
            {
                Jwt = invalidJwt
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }
    }
}
