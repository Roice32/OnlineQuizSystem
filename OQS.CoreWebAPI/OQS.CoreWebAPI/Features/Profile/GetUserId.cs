using Carter;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class GetUsername
{
    public record Command(string Jwt) : IRequest<string>;

    public class Handler : IRequestHandler<Command, string>
    {
        public Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(request.Jwt);

            var usernameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
            return Task.FromResult(usernameClaim);
        }
    }
}

public class GetUsernameEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/id", async (HttpContext context, ISender sender) =>
        {
            var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var command = new GetUsername.Command(jwt);

            var username = await sender.Send(command);
            if (!string.IsNullOrEmpty(username))
            {
                await context.Response.WriteAsync(username);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        });
    }
}
