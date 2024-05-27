using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace OQS.CoreWebAPI.Feautures.Authentication
{
    public class JwtValidator
    {
        private readonly IConfiguration configuration;

        public JwtValidator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool Validate(string jwt)
        {
            try
            {
                var secretKey = configuration["JWT:Secret"];
                var issuer = configuration["JWT:ValidIssuer"];
                var audience = configuration["JWT:ValidAudience"];

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,

                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(jwt, validationParameters, out validatedToken);


                var jwtSecurityToken = validatedToken as JwtSecurityToken;
                if (jwtSecurityToken.Issuer != validationParameters.ValidIssuer)
                {
                    Console.WriteLine("JWT issuer is not valid.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}