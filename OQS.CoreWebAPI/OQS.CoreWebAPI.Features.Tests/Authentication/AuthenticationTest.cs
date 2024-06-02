
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Xunit;
using NSubstitute;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Authentication;


namespace OQS.CoreWebAPI.Features.Authentication.Tests
{
    public class AuthenticationTest
    {
        [Fact]
        public async Task Handle_ValidAuthentication_ReturnsToken()
        {
            // Arrange
            var userStoreMock = Substitute.For<IUserStore<User>>();
            var userManagerMock = Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);
            var validatorMock = Substitute.For<IValidator<Authentication.Command>>();
            var configurationMock = Substitute.For<IConfiguration>();
            var userRoles = new List<string> { "User" };

            var command = new Authentication.Command
            {
                Username = "diana2004",
                Password = "diana1Andreea5@"
            };

            var user = new User { UserName = command.Username };
            userManagerMock.FindByNameAsync(command.Username).Returns(Task.FromResult(user));
            userManagerMock.CheckPasswordAsync(user, command.Password).Returns(Task.FromResult(true));
            userManagerMock.GetRolesAsync(user).Returns(Task.FromResult<IList<string>>(userRoles));

            validatorMock.Validate(command).Returns(new ValidationResult());

            configurationMock["JWT:Secret"].Returns("test_secret_12345678901234567890123456789012");
            configurationMock["JWT:ValidIssuer"].Returns("valid_issuer");
            configurationMock["JWT:ValidAudience"].Returns("valid_audience");

            var handler = new Authentication.Handler(
                userManagerMock,
                validatorMock,
                configurationMock);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

        }

        [Fact]
        public async Task Handle_InvalidAuthentication_ReturnsFailure()
        {
            // Arrange
            var userStoreMock = Substitute.For<IUserStore<User>>();
            var userManagerMock = Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);
            var validatorMock = Substitute.For<IValidator<Authentication.Command>>();

            var validationResult = new ValidationResult(new[] { new ValidationFailure("", "Invalid username or password") });
            validatorMock.Validate(Arg.Any<Authentication.Command>()).Returns(validationResult);

            var configurationMock = Substitute.For<IConfiguration>();

            var request = new Authentication.Command
            {
                Username = "testuser",
                Password = "invalidpassword"
            };

            userManagerMock.FindByNameAsync(request.Username).Returns(Task.FromResult<User>(null));

            var handler = new Authentication.Handler(
                userManagerMock,
                validatorMock,
                configurationMock
            );

            // Act
            var result = await handler.Handle(request, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Authentication.Validator", result.Error.Code);
            Assert.Equal("Invalid username or password", result.Error.Message);
        }
    }
}

