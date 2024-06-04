using Xunit;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.ResetPassword;
using System.Collections.Generic;

namespace OQS.CoreWebAPI.Features.ResetPassword.Tests
{
    public class ResetPasswordTest
    {
        [Fact]
        public async Task Handle_ValidResetPassword_ReturnsSuccess()
        {
            // Arrange
            var userStoreMock = Substitute.For<IUserStore<User>>();
            var userManagerMock = Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);
            var validatorMock = Substitute.For<IValidator<ResetPassword.Command>>();
            var userRole = new List<string> { "User" }; // Simulăm un utilizator cu rolul "User"

            var command = new ResetPassword.Command
            {
                Token = "valid_token",
                NewPassword = "new_password",
                Username = "existing_user"
            };

            // Simulăm un utilizator existent
            var user = new User { UserName = command.Username };
            userManagerMock.FindByNameAsync(command.Username).Returns(user);
            userManagerMock.VerifyUserTokenAsync(user, Arg.Any<string>(), Arg.Any<string>(), command.Token).Returns(true);
            userManagerMock.ResetPasswordAsync(user, command.Token, command.NewPassword).Returns(IdentityResult.Success);

            validatorMock.Validate(command).Returns(new ValidationResult());

            var handler = new ResetPassword.Handler(
                userManagerMock,
                null, //we don't use RoleManager in ResetPassword.Handler
                validatorMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_InvalidResetPassword_ReturnsFailure()
        {
            // Arrange
            var userStoreMock = Substitute.For<IUserStore<User>>();
            var userManagerMock = Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);
            var validatorMock = Substitute.For<IValidator<ResetPassword.Command>>();

            var command = new ResetPassword.Command
            {
                Token = "invalid_token",
                NewPassword = "new_password",
                Username = "nonexistent_user"
            };

            userManagerMock.FindByNameAsync(command.Username).Returns((User)null);
            userManagerMock.VerifyUserTokenAsync(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<string>(), command.Token).Returns(false);

            var validationResult = new ValidationResult(new[] { new ValidationFailure("", "Invalid token") });
            validatorMock.Validate(command).Returns(validationResult);

            var handler = new ResetPassword.Handler(
                userManagerMock,
                null, // RoleManager isn't used in ResetPassword.Handler
                validatorMock
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
