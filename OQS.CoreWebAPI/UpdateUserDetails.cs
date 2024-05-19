using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Profile;
using OQS.CoreWebAPI.Shared;
using Xunit;

namespace OQS.CoreWebAPI.Features.Tests
{
    public class UpdateUserDetailsTest
    {
        [Fact]
        public async Task Handle_ValidUpdate_ReturnsSuccess()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var validatorMock = new Mock<IValidator<UpdateUserDetails.Command>>();

            var command = new UpdateUserDetails.Command
            {
                UserId = "123",
                UpdateModel = new UpdateUserDetails.UpdateUserDetailsModel
                {
                    Username = "newusername",
                    FirstName = "NewFirstName",
                    LastName = "NewLastName",
                    Email = "newemail@example.com"
                }
            };

            var user = new User { Id = "123", UserName = "oldusername", FirstName = "OldFirstName", LastName = "OldLastName", Email = "oldemail@example.com" };
            userManagerMock.Setup(um => um.FindByIdAsync(command.UserId)).ReturnsAsync(user);
            userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);

            validatorMock.Setup(v => v.Validate(It.IsAny<UpdateUserDetails.Command>())).Returns(new ValidationResult());

            var handler = new UpdateUserDetails.Handler(userManagerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_InvalidUpdate_ReturnsFailure()
        {
            // Arrange
            var userStoreMock = new Mock<IUserStore<User>>();
            var userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            var validatorMock = new Mock<IValidator<UpdateUserDetails.Command>>();

            var command = new UpdateUserDetails.Command
            {
                UserId = "123",
                UpdateModel = new UpdateUserDetails.UpdateUserDetailsModel
                {
                    Username = "newusername",
                    FirstName = "NewFirstName",
                    LastName = "NewLastName",
                    Email = "newemail@example.com"
                }
            };

            userManagerMock.Setup(um => um.FindByIdAsync(command.UserId)).ReturnsAsync((User)null);

            var handler = new UpdateUserDetails.Handler(userManagerMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("User not found.", result.Error.Message);
        }
    }
}
