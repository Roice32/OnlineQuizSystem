using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Profile;
using OQS.CoreWebAPI.Shared;
using Xunit;

namespace OQS.CoreWebAPI.Tests.Profile
{
    public class DeleteAccountTests
    {
        private UserManager<User> CreateMockUserManager()
        {
            var userStoreMock = Substitute.For<IUserStore<User>>();
            return Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);
        }

        private IConfiguration CreateMockConfiguration()
        {
            var configurationMock = Substitute.For<IConfiguration>();
            configurationMock["JWT:Secret"].Returns("test_secret_12345678901234567890123456789012");
            configurationMock["JWT:ValidIssuer"].Returns("valid_issuer");
            configurationMock["JWT:ValidAudience"].Returns("valid_audience");
            return configurationMock;
        }

        private IValidator<DeleteAccount.Command> CreateMockValidator()
        {
            var validatorMock = Substitute.For<IValidator<DeleteAccount.Command>>();
            validatorMock.Validate(Arg.Any<DeleteAccount.Command>()).Returns(new ValidationResult());
            return validatorMock;
        }

        private string GenerateJwtToken(string issuer, string audience, string secret)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Fact]
        public async Task Handle_ValidDeleteAccount_ReturnsSuccess()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();
            var validatorMock = CreateMockValidator();

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            var command = new DeleteAccount.Command
            {
                Id = Guid.NewGuid(),
                Jwt = jwt
            };

            // Simulate an existing user
            var user = new User { Id = command.Id.ToString() };
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns(user);
            userManagerMock.DeleteAsync(user).Returns(IdentityResult.Success);

            var handler = new DeleteAccount.Handler(
                userManagerMock,
                validatorMock,
                configurationMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }



        [Fact]
        public async Task Handle_ValidationFailure_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();

            var validatorMock = Substitute.For<IValidator<DeleteAccount.Command>>();
            var validationResult = new ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Id", "Id is required")
            });
            validatorMock.Validate(Arg.Any<DeleteAccount.Command>()).Returns(validationResult);

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            var command = new DeleteAccount.Command
            {
                Id = Guid.NewGuid(),
                Jwt = jwt
            };

            var handler = new DeleteAccount.Handler(
                userManagerMock,
                validatorMock,
                configurationMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Id is required", result.Error.Message);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();
            var validatorMock = CreateMockValidator();

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            var command = new DeleteAccount.Command
            {
                Id = Guid.NewGuid(),
                Jwt = jwt
            };

            // Simulate a non-existing user
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns((User)null);

            var handler = new DeleteAccount.Handler(
                userManagerMock,
                validatorMock,
                configurationMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User doesn't exists.", result.Error.Message);
        }

        [Fact]
        public async Task Handle_DeleteFailure_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();
            var validatorMock = CreateMockValidator();

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            var command = new DeleteAccount.Command
            {
                Id = Guid.NewGuid(),
                Jwt = jwt
            };

            // Simulate an existing user
            var user = new User { Id = command.Id.ToString() };
            userManagerMock.FindByIdAsync(command.Id.ToString()).Returns(user);
            userManagerMock.DeleteAsync(user).Returns(IdentityResult.Failed(new IdentityError { Description = "Delete failed" }));

            var handler = new DeleteAccount.Handler(
                userManagerMock,
                validatorMock,
                configurationMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Delete failed", result.Error.Message);
        }
    }
}
