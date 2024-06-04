using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Profile;
using Xunit;

namespace OQS.CoreWebAPI.Feautures.Tests.Profile
{
    public class UpdateUserTest
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

        private string GenerateJwtToken(string issuer, string audience, string secret, string userId, string role)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new System.Security.Claims.Claim("sub", userId), 
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
        public async Task Handle_ValidUserUpdate_ReturnsSuccess()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();

            // Mock the expected behavior of UserManager
            userManagerMock.FindByIdAsync(Arg.Any<string>())
                .Returns(Task.FromResult(new User())); 

            userManagerMock.UpdateAsync(Arg.Any<User>())
                .Returns(Task.FromResult(IdentityResult.Success)); 

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012", "testUser", "Admin");

            var handler = new UpdateUser.Handler(userManagerMock, configurationMock);

            var command = new UpdateUser.Command
            {
                Id = Guid.NewGuid(),
                FirstName = "NewFirstName",
                LastName = "NewLastName",
                Username = "NewUsername",
                Email = "newemail@example.com",
                Jwt = jwt 
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess, result.Error.Message);
        }

        [Fact]
        public async Task Handle_InvalidJwt_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();

            var handler = new UpdateUser.Handler(userManagerMock, configurationMock);

            var command = new UpdateUser.Command
            {
                Jwt = "invalid_jwt_token" // Provide an invalid JWT token for testing
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }
    }
}
