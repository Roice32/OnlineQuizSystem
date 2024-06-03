using Xunit;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.ResetPassword;
using OQS.CoreWebAPI.Shared;
using System;

namespace OQS.CoreWebAPI.Features.ResetPassword.Tests
{
    public class ResetPasswordFromProfileTest
    {
        [Fact]
        public async Task Handle_ValidResetPassword_ReturnsSuccess()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var passwordHasherMock = Substitute.For<IPasswordHasher<User>>();
            var validatorMock = Substitute.For<IValidator<ResetPasswordFromProfile.Command>>();

            var command = new ResetPasswordFromProfile.Command
            {
                Id = Guid.NewGuid(),
                NewPassword = "new_password"
            };

            // Simulate an existing user
            var user = new User { Id = command.Id.ToString() };
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns(Task.FromResult(user));
            passwordHasherMock.HashPassword(user, command.NewPassword).Returns("hashed_password");
            userManagerMock.UpdateAsync(user).Returns(Task.FromResult(IdentityResult.Success));

            validatorMock.Validate(command).Returns(new ValidationResult());

            var handler = new ResetPasswordFromProfile.Handler(
                userManagerMock,
                passwordHasherMock
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
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var passwordHasherMock = Substitute.For<IPasswordHasher<User>>();
            var validatorMock = Substitute.For<IValidator<ResetPasswordFromProfile.Command>>();

            var command = new ResetPasswordFromProfile.Command
            {
                Id = Guid.NewGuid(),
                NewPassword = "new_password"
            };

            // Simulate no user found
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns((User)null);

            var validationResult = new ValidationResult(new[] { new ValidationFailure("", "User not found") });
            validatorMock.Validate(command).Returns(validationResult);

            var handler = new ResetPasswordFromProfile.Handler(
                userManagerMock,
                passwordHasherMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            // Assert.NotNull(result.Error);
            Assert.Equal("User not found", result.Error.Message);
        }
    }
}
