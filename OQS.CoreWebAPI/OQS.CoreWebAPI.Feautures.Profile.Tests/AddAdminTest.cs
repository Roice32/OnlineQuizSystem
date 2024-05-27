using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Profile;
using OQS.CoreWebAPI.Feautures.Authentication;
using OQS.CoreWebAPI.Shared;
using Xunit;

namespace OQS.CoreWebAPI.Feautures.Profile.Tests
{
    public class AddAdminTest
    {
        private UserManager<User> CreateMockUserManager()
        {
            var userStoreMock = Substitute.For<IUserStore<User>>();
            return Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);
        }

        private RoleManager<IdentityRole> CreateMockRoleManager()
        {
            var roleStoreMock = Substitute.For<IRoleStore<IdentityRole>>();
            return Substitute.For<RoleManager<IdentityRole>>(roleStoreMock, null, null, null, null);
        }

        private IEmailSender CreateMockEmailSender()
        {
            return Substitute.For<IEmailSender>();
        }

        private IConfiguration CreateMockConfiguration()
        {
            var configurationMock = Substitute.For<IConfiguration>();
            configurationMock["JWT:Secret"].Returns("test_secret_12345678901234567890123456789012");
            configurationMock["JWT:ValidIssuer"].Returns("valid_issuer");
            configurationMock["JWT:ValidAudience"].Returns("valid_audience");
            return configurationMock;
        }

        private IValidator<AddAdmin.Command> CreateMockValidator()
        {
            var validatorMock = Substitute.For<IValidator<AddAdmin.Command>>();
            validatorMock.Validate(Arg.Any<AddAdmin.Command>()).Returns(new FluentValidation.Results.ValidationResult());
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
        public async Task Handle_ValidAdminCreation_ReturnsSuccess()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var roleManagerMock = CreateMockRoleManager();
            var emailSenderMock = CreateMockEmailSender();
            var configurationMock = CreateMockConfiguration();
            var validatorMock = CreateMockValidator();

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            userManagerMock.CreateAsync(Arg.Any<User>(), Arg.Any<string>())
                .Returns(IdentityResult.Success);
            userManagerMock.FindByNameAsync(Arg.Any<string>())
                .Returns(Task.FromResult<User>(null));

            roleManagerMock.RoleExistsAsync(Arg.Any<string>())
                .Returns(Task.FromResult(false));
            roleManagerMock.CreateAsync(Arg.Any<IdentityRole>())
                .Returns(IdentityResult.Success);

            var handler = new AddAdmin.Handler(userManagerMock, roleManagerMock, validatorMock, emailSenderMock, configurationMock);

            var command = new AddAdmin.Command
            {
                Username = "adminUser",
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@example.com",
                Password = "Password123!",
                Jwt = jwt
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
        }

        //Failure due to invalid JWT.
        [Fact]
        public async Task Handle_InvalidJwt_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var roleManagerMock = CreateMockRoleManager();
            var emailSenderMock = CreateMockEmailSender();
            var configurationMock = CreateMockConfiguration();
            var validatorMock = CreateMockValidator();

            var invalidJwt = "invalid_jwt_token";

            var handler = new AddAdmin.Handler(userManagerMock, roleManagerMock, validatorMock, emailSenderMock, configurationMock);

            var command = new AddAdmin.Command
            {
                Username = "adminUser",
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@example.com",
                Password = "Password123!",
                Jwt = invalidJwt
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }

        //Failure due to validation error.
        [Fact]
        public async Task Handle_ValidationFailure_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var roleManagerMock = CreateMockRoleManager();
            var emailSenderMock = CreateMockEmailSender();
            var configurationMock = CreateMockConfiguration();

            var validatorMock = Substitute.For<IValidator<AddAdmin.Command>>();
            var validationResult = new FluentValidation.Results.ValidationResult(new[]
            {
                new FluentValidation.Results.ValidationFailure("Username", "Username is required")
            });
            validatorMock.Validate(Arg.Any<AddAdmin.Command>()).Returns(validationResult);

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            var handler = new AddAdmin.Handler(userManagerMock, roleManagerMock, validatorMock, emailSenderMock, configurationMock);

            var command = new AddAdmin.Command
            {
                Username = "adminUser",
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@example.com",
                Password = "Password123!",
                Jwt = jwt
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Username is required", result.Error.Message);
        }

        //Failure due to existing admin.
        [Fact]
        public async Task Handle_AdminAlreadyExists_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = CreateMockUserManager();
            var roleManagerMock = CreateMockRoleManager();
            var emailSenderMock = CreateMockEmailSender();
            var configurationMock = CreateMockConfiguration();
            var validatorMock = CreateMockValidator();

            var jwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            userManagerMock.FindByNameAsync(Arg.Any<string>())
                .Returns(Task.FromResult(new User()));

            var handler = new AddAdmin.Handler(userManagerMock, roleManagerMock, validatorMock, emailSenderMock, configurationMock);

            var command = new AddAdmin.Command
            {
                Username = "existingAdmin",
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@example.com",
                Password = "Password123!",
                Jwt = jwt
            };

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Admin already exists", result.Error.Message);
        }
    }
}
