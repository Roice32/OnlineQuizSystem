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
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Features.Profile;
using OQS.CoreWebAPI.Shared;
using Xunit;

namespace OQS.CoreWebAPI.Features.Tests.Profile
{
    public class DeleteUserTests
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

        private IValidator<DeleteUser.Command> CreateMockValidator()
        {
            var validatorMock = Substitute.For<IValidator<DeleteUser.Command>>();
            validatorMock.Validate(Arg.Any<DeleteUser.Command>()).Returns(new ValidationResult());
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

        private JwtValidator CreateMockJwtValidator(IConfiguration configuration)
        {
            var jwtValidatorMock = Substitute.For<JwtValidator>(configuration);
            jwtValidatorMock.Validate(Arg.Any<string>()).Returns(true);
            jwtValidatorMock.IsAdmin().Returns(true);
            return jwtValidatorMock;
        }

      /*  [Fact]
        public async Task Handle_ValidDeleteUser_ReturnsSuccess()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();
            var validatorMock = CreateMockValidator();
            var jwtValidatorMock = CreateMockJwtValidator(configurationMock);

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            var command = new DeleteUser.Command
            {
                Username = "testuser",
                Jwt = jwt
            };

            // Simulate an existing user
            var user = new User { UserName = command.Username };
            userManagerMock.FindByNameAsync(command.Username).Returns(Task.FromResult(user));
            userManagerMock.DeleteAsync(user).Returns(IdentityResult.Success);

            var handler = new DeleteUser.Handler(userManagerMock, validatorMock, configurationMock);
            typeof(DeleteUser.Handler)
                .GetProperty("JwtValidator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(handler, jwtValidatorMock);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess, result.Error?.Message);
        }*/


        [Fact]
        public async Task Handle_ValidationFailure_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();

            var validatorMock = Substitute.For<IValidator<DeleteUser.Command>>();
            var validationResult = new ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Username", "Username is required")
            });
            validatorMock.Validate(Arg.Any<DeleteUser.Command>()).Returns(validationResult);

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            var command = new DeleteUser.Command
            {
                Username = "testuser",
                Jwt = jwt
            };

            var handler = new DeleteUser.Handler(
                userManagerMock,
                validatorMock,
                configurationMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();
            var validatorMock = CreateMockValidator();

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            var command = new DeleteUser.Command
            {
                Username = "nonexistinguser",
                Jwt = jwt
            };

            // Simulate a non-existing user
            userManagerMock.FindByNameAsync(command.Username).Returns((User)null);

            var handler = new DeleteUser.Handler(
                userManagerMock,
                validatorMock,
                configurationMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("You are not an admin.", result.Error.Message);
        }

        [Fact]
        public async Task Handle_DeleteFailure_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var configurationMock = CreateMockConfiguration();
            var validatorMock = CreateMockValidator();

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            var command = new DeleteUser.Command
            {
                Username = "testuser",
                Jwt = jwt
            };

            // Simulate an existing user
            var user = new User { UserName = command.Username };
            userManagerMock.FindByNameAsync(command.Username).Returns(user);
            userManagerMock.DeleteAsync(user).Returns(IdentityResult.Failed(new IdentityError { Description = "Delete failed" }));

            var handler = new DeleteUser.Handler(
                userManagerMock,
                validatorMock,
                configurationMock
            );

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
           
        }
    }
}
