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
using System.Web;

namespace OQS.CoreWebAPI.Features.Authentication
{
    public class ResetPassword
    {
        public record Command : IRequest<Result>
        {
            public string Token { get; set; }
            public string NewPassword { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.NewPassword).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly UserManager<User> userManager;
            private readonly RoleManager<IdentityRole> roleManager;
            private readonly IValidator<Command> validator;

            public Handler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IValidator<Command> validator)
            {
                this.userManager = userManager;
                this.roleManager = roleManager;
                this.validator = validator;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<Guid>(
                        new Error("ResetPassword.Validator", validationResult.ToString()));
                }

                var decodedToken = HttpUtility.UrlDecode(request.Token);
                var user = await userManager.FindByIdAsync(decodedToken);

                if (user == null)
                {
                    return Result.Failure<String>(
                                               new Error("ResetPassword.Handler", "User doesn't exists."));
                }

                //var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

                if (!result.Succeeded)
                {
                    return Result.Failure(
                        new Error("ResetPassword.Handler", "Failed to reset password."));
                }

                return Result.Success();
            }
        }

    }
}
public class ResetPasswordEndPoind : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("api/resetPasssword/{token}", async (ResetPasswordModel model, ISender sender, string token) =>
        {
            var command = new ResetPassword.Command
            {
                Token = token,
                NewPassword = model.newPassword
            };


            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(new { message = "Password reset successfully!!" });
            }
            else
            {
                return Results.Ok(new { message = result.Error.Message });
            }
        });
    }
}