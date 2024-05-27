using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Profile;
using OQS.CoreWebAPI.Shared;
using Xunit;
using NSubstitute.Extensions;

namespace OQS.CoreWebAPI.Tests.Profile
{
    public class GetAllUsersTests
    {
        private UserManager<User> CreateMockUserManager(List<User> users)
        {
            var userStoreMock = Substitute.For<IQueryableUserStore<User>>();
            var userManager = Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);

            // Configure userStoreMock to work with IQueryableUserStore<User>
            userStoreMock.Users.Returns(users.AsQueryable());

            return userManager;
        }

        private IConfiguration CreateMockConfiguration()
        {
            var configurationMock = Substitute.For<IConfiguration>();
            configurationMock["JWT:Secret"].Returns("test_secret_12345678901234567890123456789012");
            configurationMock["JWT:ValidIssuer"].Returns("valid_issuer");
            configurationMock["JWT:ValidAudience"].Returns("valid_audience");
            return configurationMock;
        }

        private string GenerateJwtToken(string issuer, string audience, string secret)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

       /* [Fact]
        public async Task Handle_ValidJwt_ReturnsUserList()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserName = "user1" },
                new User { UserName = "user2" }
            };

            var userManagerMock = CreateMockUserManager(users);
            var configurationMock = CreateMockConfiguration();

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            var query = new GetAllUsers.Query
            {
                Jwt = jwt
            };

            var handler = new GetAllUsers.Handler(userManagerMock,
                                                  configurationMock);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(users.Count, result.Value.Count);
        }*/

        [Fact]
        public async Task Handle_InvalidJwt_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager(new List<User>());
            var configurationMock = CreateMockConfiguration();

            var invalidJwt = "invalid_jwt_token";

            var query = new GetAllUsers.Query
            {
                Jwt = invalidJwt
            };

            var handler = new GetAllUsers.Handler(userManagerMock, configurationMock);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }
    }
}
