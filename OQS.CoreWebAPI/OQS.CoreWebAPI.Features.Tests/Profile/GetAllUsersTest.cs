using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Profile;
using Xunit;

namespace OQS.CoreWebAPI.Features.Tests.Profile
{
    public class GetAllUsersTest
    { 
            private UserManager<User> CreateMockUserManager(IEnumerable<User> users)
            {
                var userStoreMock = Substitute.For<IUserStore<User>>();
                var userManagerMock = Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);

                userManagerMock.Users.Returns(users.AsQueryable());
                return userManagerMock;
            }

            private IConfiguration CreateMockConfiguration(string validIssuer, string validAudience, string secret)
            {
                var configurationMock = Substitute.For<IConfiguration>();
                configurationMock["JWT:Secret"].Returns(secret);
                configurationMock["JWT:ValidIssuer"].Returns(validIssuer);
                configurationMock["JWT:ValidAudience"].Returns(validAudience);
                return configurationMock;
            }

        private string GenerateJwtToken(string issuer, string audience, string secret, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Role, role)
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
            public async Task Handle_ValidRequest_ReturnsSuccess()
            {
                // Arrange
                var users = new List<User>
            {
                new User { Id = Guid.NewGuid().ToString(), UserName = "testUser1" },
                new User { Id = Guid.NewGuid().ToString(), UserName = "testUser2" },
            };

                var userManagerMock = CreateMockUserManager(users);
                var configurationMock = CreateMockConfiguration("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");
                var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012", "Admin");

                var handler = new GetAllUsers.Handler(userManagerMock, configurationMock);

                var query = new GetAllUsers.Query
                {
                    Jwt = jwt
                };

                // Act
                var result = await handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Equal(users.Count, result.Value.Count);
            }
        */
           

    [Fact]
        public async Task Handle_InvalidJwt_ReturnsFailure()
        {
            // Arrange
            var configurationMock = CreateMockConfiguration("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");
            var userManagerMock = Substitute.For<UserManager<User>>(Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var handler = new GetAllUsers.Handler(userManagerMock, configurationMock);

            var query = new GetAllUsers.Query
            {
                Jwt = "invalid_jwt_token"
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }
    }
}
