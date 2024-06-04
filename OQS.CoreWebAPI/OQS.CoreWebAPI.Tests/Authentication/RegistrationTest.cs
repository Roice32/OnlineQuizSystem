using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Authentication;
using Xunit;

namespace OQS.CoreWebAPI.Features.Authentication.Tests
{
    public class RegistrationTest
    {
        [Fact]
        public async Task Handle_ValidRegistration_ReturnsSuccess()
        {
            // Arrange
            var userStoreMock = Substitute.For<IUserStore<User>>();
            var userManagerMock = Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);

            userManagerMock.CreateAsync(Arg.Any<User>(), Arg.Any<string>())
                           .Returns(Task.FromResult(IdentityResult.Success));

            userManagerMock.FindByNameAsync(Arg.Any<string>())
                           .Returns(Task.FromResult<User>(null));

            userManagerMock.AddToRoleAsync(Arg.Any<User>(), Arg.Any<string>())
                           .Returns(Task.FromResult(IdentityResult.Success));

            var roleStoreMock = Substitute.For<IRoleStore<IdentityRole>>();
            var roleManagerMock = Substitute.For<RoleManager<IdentityRole>>(
                roleStoreMock, null, null, null, null);

            var validatorMock = Substitute.For<IValidator<Registration.Command>>();
            var emailServiceMock = Substitute.For<IEmailSender>();

            var request = new Registration.Command
            {
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe",
                Email = "test@example.com",
                Password = "Password123"
            };

            validatorMock.Validate(request)
                         .Returns(new ValidationResult());

            var handler = new Registration.Handler(
                userManagerMock,
                roleManagerMock,
                validatorMock,
                emailServiceMock
            );

            // Act
            var result = await handler.Handle(request, default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
        }


        [Fact]
        public async Task Handle_InvalidRegistration_ReturnsFailure()
        {
            // Arrange
            var userStoreMock = Substitute.For<IUserStore<User>>();
            var userManagerMock = Substitute.For<UserManager<User>>(userStoreMock, null, null, null, null, null, null, null, null);

            var roleStoreMock = Substitute.For<IRoleStore<IdentityRole>>();
            var roleManagerMock = Substitute.For<RoleManager<IdentityRole>>(
                roleStoreMock, null, null, null, null);

            var validatorMock = Substitute.For<IValidator<Registration.Command>>();
            var emailServiceMock = Substitute.For<IEmailSender>();

            var request = new Registration.Command
            {
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe",
                Email = "invalidemail", // Invalid email
                Password = "Password123"
            };

            var validationResult = new ValidationResult(new[] { new ValidationFailure("Email", "Invalid email") });
            validatorMock.Validate(Arg.Any<Registration.Command>())
                         .Returns(validationResult);

            var handler = new Registration.Handler(
                userManagerMock,
                roleManagerMock,
                validatorMock,
                emailServiceMock
            );

            // Act
            var result = await handler.Handle(request, default);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Registration.Validator", result.Error.Code);
            Assert.Equal("Invalid email", result.Error.Message);
        }
    }
}
