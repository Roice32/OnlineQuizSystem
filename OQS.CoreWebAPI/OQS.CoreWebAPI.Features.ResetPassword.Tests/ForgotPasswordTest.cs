using Xunit;
using Moq;
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
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var validatorMock = new Mock<IValidator<ForgotPassword.Command>>();
            var emailSenderMock = new Mock<IEmailSender>();
            var configurationMock = new Mock<IConfiguration>();

            var command = new ForgotPassword.Command
            {
                Email = "test@gmail.com",
                Username = "existing_user"
            };

            // Simulate a user exists
            var user = new User { UserName = command.Username, Email = command.Email };
            userManagerMock.Setup(um => um.FindByNameAsync(command.Username)).ReturnsAsync(user);

            // Setup validation to succeed
            validatorMock.Setup(v => v.Validate(command)).Returns(new ValidationResult());

            // Setup email sender
            emailSenderMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(Task.CompletedTask);

            var handler = new ForgotPassword.Handler(
                userManagerMock.Object,
                validatorMock.Object,
                configurationMock.Object,
                emailSenderMock.Object
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
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var validatorMock = new Mock<IValidator<ForgotPassword.Command>>();
            var emailSenderMock = new Mock<IEmailSender>();
            var configurationMock = new Mock<IConfiguration>();

            var command = new ForgotPassword.Command
            {
                Email = "test@gmail.com",
                Username = "nonexistent_user"
            };

            // Simulate no user found
            userManagerMock.Setup(um => um.FindByNameAsync(command.Username)).ReturnsAsync((User)null);

            // Setup validation to fail
            var validationResult = new ValidationResult(new[] { new ValidationFailure("", "Invalid username") });
            validatorMock.Setup(v => v.Validate(command)).Returns(validationResult);

            var handler = new ForgotPassword.Handler(
                userManagerMock.Object,
                validatorMock.Object,
                configurationMock.Object,
                emailSenderMock.Object
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

