using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Authentication;
using OQS.CoreWebAPI.Shared;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Feautures.Authentication
{
    public class Logout
    {
        public record Command : IRequest<Result>
        {
            public string Jwt { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly SignInManager<User> signInManager;
            private readonly IConfiguration configuration;

            public Handler(SignInManager<User> signInManager, IConfiguration configuration)
            {
                this.signInManager = signInManager;
                this.configuration = configuration;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var jwtValidator = new JwtValidator(configuration);
                if (!jwtValidator.Validate(request.Jwt))
                {
                    return Result.Failure<List<User>>(
                        new Error("Authentication", "Invalid Jwt"));
                }

                await signInManager.SignOutAsync();

                return Result.Success();
            }
        }
    }
}

public class LogoutEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("api/profile/{id}/logout", async (HttpContext context, ISender sender) =>
        {
            var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var command = new Logout.Command
            {
                Jwt = jwt
            };

            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(new { message = "User logged out successfully!" });
            }
            else
            {
                return Results.Ok(new { message = result.Error.Message });
            }
        });
    }
}