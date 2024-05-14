using Carter;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class UserName : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/username", async (HttpContext context) =>
        {
            var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var username = GetUsernameFromJWT(jwt);
            if (!string.IsNullOrEmpty(username))
            {
                await context.Response.WriteAsync(username);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        });
    }

    private string GetUsernameFromJWT(string jwt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(jwt);

        var usernameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
        return usernameClaim;
    }
}
