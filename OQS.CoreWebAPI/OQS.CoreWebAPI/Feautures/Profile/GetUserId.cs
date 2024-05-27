using Carter;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Authentication;
using OQS.CoreWebAPI.Feautures.Authentication;
using OQS.CoreWebAPI.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace QOS.CoreWebAPI.Feautures.Profile
{
    public class GetUserId
    {
        public record Command : IRequest<Result<GetUserIdResponse>>
        {
            public string Jwt { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<GetUserIdResponse>>
        {
            private readonly IConfiguration configuration;

            public Handler(IConfiguration configuration)
            {
                this.configuration = configuration;
            }

            public async Task<Result<GetUserIdResponse>> Handle(Command request, CancellationToken cancellationToken)
            {
                var jwtValidator = new JwtValidator(configuration);
                if (!jwtValidator.Validate(request.Jwt))
                {
                    return Result.Failure<GetUserIdResponse>(
                        new Error("Authentication", "Invalid Jwt"));
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(request.Jwt);

                var usernameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
                var roleClaim = token.Claims.FirstOrDefault(claim => claim.Type == "role")?.Value;

                var response = new GetUserIdResponse
                {
                    Id = usernameClaim,
                    Role = roleClaim
                };
                return Result.Success(response);
            }
        }
    }
}


public class GetUserIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/id", async (HttpContext context, ISender sender) =>
        {
            var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var command = new QOS.CoreWebAPI.Feautures.Profile.GetUserId.Command
            {
                Jwt = jwt
            };

            var response = await sender.Send(command);
            if (response.IsSuccess)
            {
                return Results.Ok(response.Value);
            }
            return Results.Ok(new { message = response.Error.Message });

        });
    }
}