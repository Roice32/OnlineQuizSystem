using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Shared;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Features.Authentication
{
    public class Logout
    {
        public record Command : IRequest<Result>
        {
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly SignInManager<User> signInManager;

            public Handler(SignInManager<User> signInManager)
            {
                this.signInManager = signInManager;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
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
        _ = app.MapPost("api/profile/{id}/logout", async (ISender sender) =>
        {
            var command = new Logout.Command();

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
