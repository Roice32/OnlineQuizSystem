using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using OQS.CoreWebAPI.Features.Authentication;
using Xunit;

namespace OQS.CoreWebAPI.Features.Authentication.Tests
{
    public class JwtValidatorTests
    {
        [Fact]
        public void Validate_ValidJwt_ReturnsTrue()
        {
            // Arrange
            var configurationMock = Substitute.For<IConfiguration>();
            configurationMock["JWT:Secret"].Returns("test_secret_12345678901234567890123456789012");
            configurationMock["JWT:ValidIssuer"].Returns("valid_issuer");
            configurationMock["JWT:ValidAudience"].Returns("valid_audience");

            var jwtValidator = new JwtValidator(configurationMock);
            var validJwt = GenerateJwtToken("valid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            // Act
            var result = jwtValidator.Validate(validJwt);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Validate_InvalidJwt_ReturnsFalse()
        {
            // Arrange
            var configurationMock = Substitute.For<IConfiguration>();
            configurationMock["JWT:Secret"].Returns("test_secret_12345678901234567890123456789012");
            configurationMock["JWT:ValidIssuer"].Returns("valid_issuer");
            configurationMock["JWT:ValidAudience"].Returns("valid_audience");

            var jwtValidator = new JwtValidator(configurationMock);
            var invalidJwt = "invalid_jwt_token";

            // Act
            var result = jwtValidator.Validate(invalidJwt);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Validate_JwtWithInvalidIssuer_ReturnsFalse()
        {
            // Arrange
            var configurationMock = Substitute.For<IConfiguration>();
            configurationMock["JWT:Secret"].Returns("test_secret_12345678901234567890123456789012");
            configurationMock["JWT:ValidIssuer"].Returns("valid_issuer");
            configurationMock["JWT:ValidAudience"].Returns("valid_audience");

            var jwtValidator = new JwtValidator(configurationMock);
            var invalidIssuerJwt = GenerateJwtToken("invalid_issuer", "valid_audience", "test_secret_12345678901234567890123456789012");

            // Act
            var result = jwtValidator.Validate(invalidIssuerJwt);

            // Assert
            Assert.False(result);
        }

        private string GenerateJwtToken(string issuer, string audience, string secret)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

