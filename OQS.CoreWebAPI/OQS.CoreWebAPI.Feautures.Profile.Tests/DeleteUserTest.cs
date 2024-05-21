using Xunit;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Feautures.Profile;
using System.ComponentModel.DataAnnotations;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Tests.Profile
{
    public class DeleteUserTests
    {
        [Fact]
        public async Task Handle_ValidDeleteUser_ReturnsSuccess()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var validatorMock = Substitute.For<IValidator<DeleteUser.Command>>();

            var command = new DeleteUser.Command
            {
                Id = Guid.NewGuid()
            };

            // Simulate an existing user
            var user = new User { Id = command.Id.ToString() };
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns(user);
            userManagerMock.DeleteAsync(user).Returns(IdentityResult.Success);

            validatorMock.Validate(command).Returns(new FluentValidation.Results.ValidationResult());

            var handler = new DeleteUser.Handler(
                userManagerMock,
                validatorMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_InvalidDeleteUser_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = Substitute.For<UserManager<User>>(
                Substitute.For<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var validatorMock = Substitute.For<IValidator<DeleteUser.Command>>();

            var command = new DeleteUser.Command
            {
                Id = Guid.NewGuid()
            };

            // Simulate a non-existing user
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns((User)null);

            var validationResult = new FluentValidation.Results.ValidationResult(new[] { new ValidationFailure("", "User not found!") });
            validatorMock.Validate(command).Returns(validationResult);

            var handler = new DeleteUser.Handler(
                userManagerMock,
                validatorMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found!", result.Error.Message);
        }
    }
}

