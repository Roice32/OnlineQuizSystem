using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Authentication;
using System;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Feautures.Authentication.Tests
{
    public class RegistrationTests
    {
        [Fact]
        public async Task Handle_ValidRegistration_ReturnsSuccess()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                            .ReturnsAsync((User)null);

            userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                roleStoreMock.Object, null, null, null, null);

            var validatorMock = new Mock<IValidator<Registration.Command>>();
            var emailServiceMock = new Mock<IEmailSender>();

            var request = new Registration.Command
            {
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe",
                Email = "test@example.com",
                Password = "Password123"
            };

            validatorMock.Setup(v => v.Validate(request))
                         .Returns(new FluentValidation.Results.ValidationResult());

            var handler = new Registration.Handler(
                userManagerMock.Object,
                roleManagerMock.Object,
                validatorMock.Object,
                emailServiceMock.Object
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
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            var validatorMock = new Mock<IValidator<Registration.Command>>();
            var emailServiceMock = new Mock<IEmailSender>();

            var request = new Registration.Command
            {
                Username = "testuser",
                FirstName = "John",
                LastName = "Doe",
                Email = "invalidemail", // Email invalid
                Password = "Password123"
            };

            var validationResult = new ValidationResult(new[] { new ValidationFailure("Email", "Invalid email") });
            validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

            var handler = new Registration.Handler(
                userManagerMock.Object,
                roleManagerMock.Object,
                validatorMock.Object,
                emailServiceMock.Object
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
