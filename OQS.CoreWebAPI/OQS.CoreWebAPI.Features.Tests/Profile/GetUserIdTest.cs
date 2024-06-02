using Xunit;
using NSubstitute;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OQS.CoreWebAPI.Features.Profile;
using OQS.CoreWebAPI.Contracts.Models;


namespace OQS.CoreWebAPI.Tests.Profile
{
    public class GetUserIdTest
    {
        /*  [Fact]
          public async Task Handle_ValidJwt_ReturnsUserId()
          {
              // Arrange
              var configurationMock = Substitute.For<IConfiguration>();

              var jwt = "valid_jwt_token";

              var handler = new GetUserId.Handler(configurationMock);

              var command = new GetUserId.Command
              {
                  Jwt = jwt
              };

              // Act
              var result = await handler.Handle(command, CancellationToken.None);

              // Assert
              Assert.True(result.IsSuccess);
              Assert.NotNull(result.Value);
              Assert.Equal("username_claim_value", result.Value.Id);
              Assert.Equal("role_claim_value", result.Value.Role);
          }

        [Fact]
        public async Task Handle_InvalidJwt_ReturnsFailure()
        {
            // Arrange
            var configurationMock = Substitute.For<IConfiguration>();

            var jwt = "invalid_jwt_token";

            var handler = new GetUserId.Handler(configurationMock);

            var command = new GetUserId.Command
            {
                Jwt = jwt
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal("Invalid Jwt", result.Error.Message);
        }*/
    }
}
