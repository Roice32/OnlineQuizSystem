using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Authentication;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Feautures.Profile
{
    public static class GetUserDetails
    {
        public class Query : IRequest<Result<UserDetailsModel>>
        {
            public Guid Id { get; set; }
            public string Jwt { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<UserDetailsModel>>
        {
            private readonly UserManager<User> userManager;
            private readonly IConfiguration configuration;

            public Handler(UserManager<User> userManager, IConfiguration configuration)
            {
                this.userManager = userManager;
                this.configuration = configuration;
            }

            public async Task<Result<UserDetailsModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                var jwtValidator = new JwtValidator(configuration);
                if (!jwtValidator.Validate(request.Jwt))
                {
                    return Result.Failure<UserDetailsModel>(
                        new Error("Authentication", "Invalid Jwt"));
                }

                var user = await userManager.FindByIdAsync(request.Id.ToString());
                if (user == null)
                {
                    return Result.Failure<UserDetailsModel>(
                                               new Error("Profile.Handler", "User doesn't exists."));
                }

                var userDetails = new UserDetailsModel
                {
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                };

                return userDetails;
            }
        }

    }
}
public class GetUserDetailsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {

        app.MapGet("api/profile/{id}", async (HttpContext context, Guid id, ISender sender) =>
        {
            var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var query = new OQS.CoreWebAPI.Feautures.Profile.GetUserDetails.Query
            {
                Id = id,
                Jwt = jwt
            };
            var result = await sender.Send(query);
            if (result.IsFailure)
            {
                return Results.Ok(result.Error);
            }
            return Results.Ok(result.Value);
        }
        );
    }
}