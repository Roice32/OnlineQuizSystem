using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.Profile
{
    public class DeleteUser
    {
        public record Command : IRequest<Result<Result>>
        {
            public string Username{ get; set; }
            public string Jwt { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Username).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result<Result>>
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

            public async Task<Result<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                var jwtValidator = new JwtValidator(configuration);
                if (!jwtValidator.Validate(request.Jwt))
                {
                    return Result.Failure<List<User>>(
                        new Error("Authentication", "Invalid Jwt."));
                }

                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<Result>(
                                               new Error("DeleteUser.Validator", validationResult.ToString()));
                }

                var user = await userManager.FindByNameAsync(request.Username);
                if (user == null)
                {
                    return Result.Failure<Result>(
                                               new Error("DeleteUser.Handler", "User doesn't exists."));
                }

                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return Result.Failure<Result>(
                                               new Error("DeleteUser.Handler", result.Errors.First().Description));
                }

                return Result.Success();
            }
        }
    }
}

public class DeleteUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/profile/delete_user", async (HttpContext context, String username,  ISender sender) =>
        {
            var jwt = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var command = new OQS.CoreWebAPI.Features.Profile.DeleteUser.Command
            {
                Jwt = jwt,
                Username = username
            };

            var result = await sender.Send(command);
            if(result.IsSuccess)
            {
                return Results.Ok( new { message = "User deleted successfully!" });
            }
            return Results.Ok(new { message = result.Error.Message });
        });
    }
}
