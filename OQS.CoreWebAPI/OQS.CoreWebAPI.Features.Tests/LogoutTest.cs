using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
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

        [Fact]
        public async Task Handle_ValidLogout_ReturnsSuccess()
        {
            // Arrange
            var signInManagerMock = CreateMockSignInManager();

            signInManagerMock.SignOutAsync().Returns(Task.CompletedTask);

            var handler = new Logout.Handler(signInManagerMock);

            // Act
            var result = await handler.Handle(new Logout.Command(), default);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_InvalidLogout_ReturnsFailure()
        {
            // Arrange
            var signInManagerMock = CreateMockSignInManager();

            signInManagerMock.SignOutAsync().Throws(new Exception("Logout failed"));

            var handler = new Logout.Handler(signInManagerMock);

            // Act
            var result = await handler.Handle(new Logout.Command(), default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Logout failed", result.Error.Message);
        }
    }
}
