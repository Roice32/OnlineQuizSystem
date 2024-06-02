
using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.ResetPassword
{
    public class ResetPasswordFromProfile
    {
        public record Command : IRequest<Result>
        {
            public string NewPassword { get; set; }
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.NewPassword).NotEmpty();
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly UserManager<User> userManager;
            private readonly IPasswordHasher<User> passwordHasher;

            public Handler(UserManager<User> userManager, IPasswordHasher<User> passwordHasher)
            {
                this.userManager = userManager;
                this.passwordHasher = passwordHasher;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await userManager.FindByIdAsync(request.Id.ToString());
                if (user == null)
                {
                    return Result.Failure(new Error("ResetPasswordFromProfile", "User not found"));
                }

                var hashedPassword = passwordHasher.HashPassword(user, request.NewPassword);
                user.PasswordHash = hashedPassword;

                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Result.Failure(new Error("ResetPasswordFromProfile", "Failed to reset password"));
                }

                return Result.Success();
            }
        }
    }
}

public class ResetPasswordFromProfileEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("api/profile/{id}/reset_current_password", async (Guid id, ResetCurrentPasswordModel model, ISender sender) =>
        {
            var command = new OQS.CoreWebAPI.Features.ResetPassword.ResetPasswordFromProfile.Command
            {
                Id = id,
                NewPassword = model.NewPassword
            };

            var result = await sender.Send(command);
            if(result.IsSuccess)
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
