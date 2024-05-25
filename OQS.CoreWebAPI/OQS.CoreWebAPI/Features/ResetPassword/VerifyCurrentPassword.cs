using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.ResetPassword;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.ResetPassword
{
    public class VerifyCurrentPassword
    {
        public class VerifyCurrentPasswordCommand : IRequest<Result>
        {
            public string Password { get; set; }
            public Guid Id { get; set; }
        }

        public class VerifyCurrentPasswordValidator : AbstractValidator<VerifyCurrentPasswordCommand>
        {
            public VerifyCurrentPasswordValidator()
            {
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class VerifyCurrentPasswordHandler : IRequestHandler<VerifyCurrentPasswordCommand, Result>
        {
            private readonly UserManager<User> userManager;
            private readonly IPasswordHasher<User> passwordHasher;

            public VerifyCurrentPasswordHandler(UserManager<User> userManager, IPasswordHasher<User> passwordHasher)
            {
                this.userManager = userManager;
                this.passwordHasher = passwordHasher;
            }

            public async Task<Result> Handle(VerifyCurrentPasswordCommand request, CancellationToken cancellationToken)
            {
                var user = await userManager.FindByIdAsync(request.Id.ToString());
                if (user == null)
                {
                    return Result.Failure(new Error("VerifyCurrentPassword", "User not found"));
                }

                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    return Result.Failure(new Error("VerifyCurrentPassword", "Invalid password"));
                }

                return Result.Success();
            }
        }

    }
}

public class VerifyCurrentPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/profile/{id}/verify_current_password", async (Guid id, VerifyPasswordModel model, ISender sender) =>
        {
            var command = new VerifyCurrentPassword.VerifyCurrentPasswordCommand
            {
                Id = id,
                Password = model.Password
            };


            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(new { message = " Confirmed Password !!" });
            }
            else
            {
                return Results.Ok(new { message = result.Error.Message });
            }
        });
    }
}


