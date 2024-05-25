using Carter;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Shared;
using Microsoft.EntityFrameworkCore;

namespace OQS.CoreWebAPI.Features.Profile
{
    public static class GetAllUsers
    {
        public class Query : IRequest<Result<List<User>>>
        {
            public string Jwt { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Query, Result<List<User>>>
        {
            private readonly UserManager<User> userManager;
            private readonly IConfiguration configuration;

            public Handler(UserManager<User> userManager, IConfiguration configuration)
            {
                this.userManager = userManager;
                this.configuration = configuration;
            }

            public async Task<Result<List<User>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var jwtValidator = new JwtValidator(configuration);
                if (!jwtValidator.Validate(request.Jwt))
                {
                    return Result.Failure<List<User>>(
                        new Error("Authentication", "Invalid Jwt"));
                }

                var users = await userManager.Users.ToListAsync(cancellationToken);
                return Result.Success(users);
            }
        }
    }
}

public class GetAllUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("api/getUsers", async (HttpContext context, ISender sender) =>
        {
            var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var query = new OQS.CoreWebAPI.Features.Profile.GetAllUsers.Query
            {
                Jwt = jwt
            };

            var result = await sender.Send(query);
            if (result.IsSuccess)
            {
                return Results.Ok(result.Value);
            }
            else
            {
                return Results.Ok(result.Error);
            }
        });
    }
}
