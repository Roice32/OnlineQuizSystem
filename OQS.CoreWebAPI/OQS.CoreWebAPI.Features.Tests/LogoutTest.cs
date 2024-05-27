using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Authentication;
using Xunit;

namespace OQS.CoreWebAPI.Feautures.Authentication.Tests
{
    public class LogoutTest
    {
        private SignInManager<User> CreateMockSignInManager()
        {
            var userStoreMock = Substitute.For<IUserStore<User>>();
            var userManagerMock = Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);

            var contextAccessorMock = Substitute.For<IHttpContextAccessor>();
            var claimsFactoryMock = Substitute.For<IUserClaimsPrincipalFactory<User>>();
            var optionsMock = Substitute.For<IOptions<IdentityOptions>>();
            var loggerMock = Substitute.For<ILogger<SignInManager<User>>>();
            var schemesMock = Substitute.For<IAuthenticationSchemeProvider>();
            var confirmationMock = Substitute.For<IUserConfirmation<User>>();

            return Substitute.For<SignInManager<User>>(userManagerMock, contextAccessorMock, claimsFactoryMock, optionsMock, loggerMock, schemesMock, confirmationMock);
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
                expires: System.DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Fact]
        public async Task Handle_ValidLogout_ReturnsSuccess()
        {
            // Arrange
            var signInManagerMock = CreateMockSignInManager();
            var configurationMock = CreateMockConfiguration();

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            signInManagerMock.SignOutAsync().Returns(Task.CompletedTask);

            var handler = new Logout.Handler(signInManagerMock, configurationMock);

            // Act
            var result = await handler.Handle(new Logout.Command { Jwt = jwt }, default);

            // Assert
            Assert.True(result.IsSuccess);
        }

       
    }
}
