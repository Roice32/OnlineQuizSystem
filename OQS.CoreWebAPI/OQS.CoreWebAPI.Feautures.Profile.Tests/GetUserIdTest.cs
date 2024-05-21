using Xunit;
using NSubstitute;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace OQS.CoreWebAPI.Tests.Profile
{
    public class GetUsernameTests
    {
        private const string SecretKey = "supersecretkey!12345678901234567890123456789012"; 

        [Fact]
        public async Task Handle_ValidJwt_ReturnsUsername()
        {
            // Arrange
            var username = "testuser";
            var jwt = CreateJwtToken(username);

            var handler = new GetUsername.Handler();
            var command = new GetUsername.Command(jwt);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(username, result);
        }

        [Fact]
        public async Task Handle_InvalidJwt_ReturnsNull()
        {
            // Arrange
            var jwt = "invalid_jwt";
            var handler = new GetUsername.Handler();
            var command = new GetUsername.Command(jwt);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        private string CreateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("unique_name", username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
