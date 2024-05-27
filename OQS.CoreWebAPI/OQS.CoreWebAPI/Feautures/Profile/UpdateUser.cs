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
    public class UpdateUser
    {
        public record Command : IRequest<Result>
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Jwt { get; set; }

        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.FirstName).NotEmpty();
                RuleFor(x => x.LastName).NotEmpty();
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly UserManager<User> userManager;
            private readonly IConfiguration configuration;

            public Handler(UserManager<User> userManager, IConfiguration configuration)
            {
                this.userManager = userManager;
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

                var user = await userManager.FindByIdAsync(request.Id.ToString());
                if (user == null)
                {
                    return Result.Failure(new Error("UpdateUser.Handler", "User not found."));
                }

                user.UserName = request.Username;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Result.Success();
                }

                return Result.Failure(new Error("UpdateUser.Handler", "Failed to update user details."));
            }

        }
    }
}

public class UpdateUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/profile/{id}/edit_profile", async (Guid id, HttpContext context, UserDetailsModel model, ISender sender) =>
        {
            var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var command = new OQS.CoreWebAPI.Feautures.Profile.UpdateUser.Command
            {
                Id = id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Email = model.Email,
                Jwt = jwt
            };

            var response = await sender.Send(command);
            if (response.IsSuccess)
            {
                return Results.Ok(new { message = "Details reset successfully!" });
            }
            else
            {
                return Results.Ok(new { message = response.Error.Message });
            }
        });
    }
}