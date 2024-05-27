using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Authentication;
using OQS.CoreWebAPI.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OQS.CoreWebAPI.Feautures.Authentication
{
    public class Authentication
    {
        public record Command : IRequest<Result<String>>
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result<String>>
        {
            private readonly UserManager<User> userManager;
            private readonly IValidator<Command> validator;
            private readonly IConfiguration configuration;

            public Handler(UserManager<User> userManager, IValidator<Command> validator, IConfiguration configuration)
            {
                this.userManager = userManager;
                this.validator = validator;
                this.configuration = configuration;
            }

            public async Task<Result<String>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<String>(
                                               new Error("Authentication.Validator", validationResult.ToString()));
                }

                var user = await userManager.FindByNameAsync(request.Username!);
                if (user == null)
                {
                    return Result.Failure<String>(
                                               new Error("Authentication.Handler", "User doesn't exists."));
                }

                if (!await userManager.CheckPasswordAsync(user, request.Password!))
                {
                    return Result.Failure<String>(
                                               new Error("Authentication.Handler", "Invalid user or password."));
                }



                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Id!),
                    new Claim(ClaimTypes.Role, userRoles.FirstOrDefault()!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };



                string token = GenerateToken(authClaims);

                return Result.Success(token);
            }

            private string GenerateToken(IEnumerable<Claim> claims)
            {
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = configuration["JWT:ValidIssuer"]!,
                    Audience = configuration["JWT:ValidAudience"]!,
                    Expires = DateTime.UtcNow.AddHours(3),
                    SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                    Subject = new ClaimsIdentity(claims)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
        }
    }
}

public class AuthenticationEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("api/authentication", async (LoginModel model, ISender sender) =>
        {
            var command = new Authentication.Command
            {
                Username = model.Username,
                Password = model.Password
            };

            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(new { token = result.Value });
            }
            else
            {
                return Results.Ok(new { message = result.Error.Message });
            }
        });
    }
}