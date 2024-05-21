using Xunit;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.ResetPassword;
using OQS.CoreWebAPI.Feautures.Authentication;
using Microsoft.Extensions.Configuration;

namespace OQS.CoreWebAPI.Feautures.ResetPassword.Tests
{
    public class ForgotPasswordTest
    {
        [Fact]
        public async Task Handle_ValidForgotPassword_ReturnsSuccess()
        {
            // Arrange
            var userStoreMock = Substitute.For<IUserStore<User>>();
            var userManagerMock = Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);
            var validatorMock = Substitute.For<IValidator<ForgotPassword.Command>>();
            var emailSenderMock = Substitute.For<IEmailSender>();
            var configurationMock = Substitute.For<IConfiguration>();

            var command = new ForgotPassword.Command
            {
                Email = "test@gmail.com",
                Username = "existing_user"
            };

            // Simulate a user exists
            var user = new User { UserName = command.Username, Email = command.Email };
            userManagerMock.FindByNameAsync(command.Username).Returns(user);

            // Setup validation to succeed
            validatorMock.Validate(command).Returns(new ValidationResult());

            // Setup email sender
            emailSenderMock.SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                           .Returns(Task.CompletedTask);

            var handler = new ForgotPassword.Handler(
                userManagerMock,
                validatorMock,
                configurationMock,
                emailSenderMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_InvalidForgotPassword_ReturnsFailure()
        {
            // Arrange
            var userStoreMock = Substitute.For<IUserStore<User>>();
            var userManagerMock = Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);
            var validatorMock = Substitute.For<IValidator<ForgotPassword.Command>>();
            var emailSenderMock = Substitute.For<IEmailSender>();
            var configurationMock = Substitute.For<IConfiguration>();

            var command = new ForgotPassword.Command
            {
                Email = "test@gmail.com",
                Username = "nonexistent_user"
            };

            // Simulate no user found
            userManagerMock.FindByNameAsync(command.Username).Returns((User)null);

            // Setup validation to fail
            var validationResult = new ValidationResult(new[] { new ValidationFailure("", "Invalid username") });
            validatorMock.Validate(command).Returns(validationResult);

            var handler = new ForgotPassword.Handler(
                userManagerMock,
                validatorMock,
                configurationMock,
                emailSenderMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Invalid username", result.Error.Message);
        }
    }
}
