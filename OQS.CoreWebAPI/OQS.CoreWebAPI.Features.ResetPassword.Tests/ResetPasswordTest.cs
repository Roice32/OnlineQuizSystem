using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.ResetPassword;


namespace OQS.CoreWebAPI.Feautures.ResetPassword.Tests
{
    public class ResetPasswordTest
    {
        [Fact]
        public async Task Handle_ValidResetPassword_ReturnsSuccess()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>( userStoreMock.Object, null, null, null, null, null, null, null, null);
            var validatorMock = new Mock<IValidator<ResetPassword.Command>>();
            var userRole = new List<string> { "User" }; // Simulăm un utilizator cu rolul "User"

            var command = new ResetPassword.Command
            {
                Token = "valid_token",
                NewPassword = "new_password",
                Username = "existing_user"
            };

            // Simulăm un utilizator existent
            var user = new User { UserName = command.Username };
            userManagerMock.Setup(um => um.FindByNameAsync(command.Username)).ReturnsAsync(user);
            userManagerMock.Setup(um => um.VerifyUserTokenAsync(user, It.IsAny<string>(), It.IsAny<string>(), command.Token)).ReturnsAsync(true);
            userManagerMock.Setup(um => um.ResetPasswordAsync(user, command.Token, command.NewPassword)).ReturnsAsync(IdentityResult.Success);

            validatorMock.Setup(v => v.Validate(command)).Returns(new ValidationResult());

            var handler = new ResetPassword.Handler(
                userManagerMock.Object,
                null, //we don't use RoleManager in ResetPassword.Handler
                validatorMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert

            Assert.True(result.IsSuccess);
            //Assert.Null(result.Error);
        }

        [Fact]
        public async Task Handle_InvalidResetPassword_ReturnsFailure()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            var validatorMock = new Mock<IValidator<ResetPassword.Command>>();

            var command = new ResetPassword.Command
            {
                Token = "invalid_token",
                NewPassword = "new_password",
                Username = "nonexistent_user"
            };

            userManagerMock.Setup(um => um.FindByNameAsync(command.Username)).ReturnsAsync((User)null);
            userManagerMock.Setup(um => um.VerifyUserTokenAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>(), command.Token)).ReturnsAsync(false);

            var validationResult = new ValidationResult(new[] { new ValidationFailure("", "Invalid token") });
            validatorMock.Setup(v => v.Validate(command)).Returns(validationResult);

            var handler = new ResetPassword.Handler(
                userManagerMock.Object,
                null, // RoleManager isn't used in ResetPassword.Handler
                validatorMock.Object
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Invalid token", result.Error.Message); // or user doesn't exist
        }
    }
}
