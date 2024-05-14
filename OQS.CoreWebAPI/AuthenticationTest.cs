using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Authentication;
using Xunit;


namespace OQS.CoreWebAPI.Features.Tests
{
    public class AuthenticationTest
    {
        [Fact]
        public async Task Handle_ValidAuthentication_ReturnsToken()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<User>>();

            var userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var validatorMock = new Mock<IValidator<Authentication.Command>>();
            var configurationMock = new Mock<IConfiguration>();
            var userRoles = new List<string> { "User" }; // Simulăm un utilizator cu rolul "User"


            var command = new Authentication.Command
            {
                Username = "diana2004",
                Password = "diana1Andreea5@"
            };

            var user = new User { UserName = command.Username };
            userManagerMock.Setup(um => um.FindByNameAsync(command.Username)).ReturnsAsync(user);

            userManagerMock.Setup(um => um.CheckPasswordAsync(user, command.Password)).ReturnsAsync(true);

            userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(userRoles);


            validatorMock.Setup(v => v.Validate(command)).Returns(new FluentValidation.Results.ValidationResult());

            configurationMock.Setup(c => c["JWT:Secret"]).Returns("test_secret_12345678901234567890123456789012"); // Cheie de 32 de caract)

            configurationMock.Setup(c => c["JWT:ValidIssuer"]).Returns("valid_issuer");
            configurationMock.Setup(c => c["JWT:ValidAudience"]).Returns("valid_audience");

            var handler = new Authentication.Handler(
                userManagerMock.Object,
                validatorMock.Object,
                configurationMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.IsType<string>(result.Value);
        }



        [Fact]
        public async Task Handle_InvalidAuthentication_ReturnsFailure()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var validatorMock = new Mock<IValidator<Authentication.Command>>();

            var validationResult = new FluentValidation.Results.ValidationResult(new[] { new ValidationFailure("", "Invalid username or password") });
            validatorMock.Setup(v => v.Validate(It.IsAny<Authentication.Command>())).Returns(validationResult);

            var configurationMock = new Mock<IConfiguration>();

            var request = new Authentication.Command
            {
                Username = "testuser",
                Password = "invalidpassword"
            };

            userManagerMock.Setup(x => x.FindByNameAsync(request.Username!))
                            .ReturnsAsync((User)null);

            var handler = new Authentication.Handler(
                userManagerMock.Object,
                validatorMock.Object,
                configurationMock.Object
            );

            // Act
            var result = await handler.Handle(request, default);

            // Assert
            Assert.False(result.IsSuccess);
            //  Assert.Null(result.Value);
            Assert.NotNull(result.Error);
            Assert.Equal("Authentication.Validator", result.Error.Code);
            Assert.Equal("Invalid username or password", result.Error.Message);
        }
    }
}