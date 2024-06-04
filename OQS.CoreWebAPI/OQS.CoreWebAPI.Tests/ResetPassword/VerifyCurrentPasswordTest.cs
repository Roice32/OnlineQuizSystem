using Xunit;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.ResetPassword;

namespace OQS.CoreWebAPI.Features.ResetPassword.Tests
{
    public class VerifyCurrentPasswordTest
    {
        [Fact]
        public async Task Handle_CorrectPassword_ReturnsSuccess()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var passwordHasherMock = Substitute.For<IPasswordHasher<User>>();

            var command = new VerifyCurrentPassword.VerifyCurrentPasswordCommand
            {
                Id = Guid.NewGuid(),
                Password = "correct_password"
            };

            // Simulate an existing user
            var user = new User { Id = command.Id.ToString(), PasswordHash = "hashed_password" };
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns(user);
            passwordHasherMock.VerifyHashedPassword(user, user.PasswordHash, command.Password).Returns(PasswordVerificationResult.Success);

            var handler = new VerifyCurrentPassword.VerifyCurrentPasswordHandler(
                userManagerMock,
                passwordHasherMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

        }

        [Fact]
        public async Task Handle_IncorrectPassword_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var passwordHasherMock = Substitute.For<IPasswordHasher<User>>();

            var command = new VerifyCurrentPassword.VerifyCurrentPasswordCommand
            {
                Id = Guid.NewGuid(),
                Password = "incorrect_password"
            };

            // Simulate an existing user
            var user = new User { Id = command.Id.ToString(), PasswordHash = "hashed_password" };
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns(user);
            passwordHasherMock.VerifyHashedPassword(user, user.PasswordHash, command.Password).Returns(PasswordVerificationResult.Failed);

            var handler = new VerifyCurrentPassword.VerifyCurrentPasswordHandler(
                userManagerMock,
                passwordHasherMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Invalid password", result.Error.Message);
        }
    }
}

