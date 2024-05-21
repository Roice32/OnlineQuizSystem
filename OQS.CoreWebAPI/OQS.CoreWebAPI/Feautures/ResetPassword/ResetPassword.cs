﻿using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Authentication;
using OQS.CoreWebAPI.Feautures.ResetPassword;
using OQS.CoreWebAPI.Shared;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

namespace OQS.CoreWebAPI.Feautures.ResetPassword
{
    public class ResetPassword
    {
        public record Command : IRequest<Result>
        {
            public string Token { get; set; }
            public string NewPassword { get; set; }
            public string Username { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.NewPassword).NotEmpty();
                RuleFor(x => x.Username).NotEmpty();
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


                var decodedToken1 = HttpUtility.UrlDecode(request.Token);
                var decodedToken = decodedToken1.Replace(" ", "+"); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 
                var user = await userManager.FindByNameAsync(request.Username!);

                if (user == null)
                {
                    return Result.Failure<String>(
                        new Error("ResetPassword.Handler", "User doesn't exists."));
                }
                else
                {
                    var response = userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, UserManager<User>.ResetPasswordTokenPurpose, decodedToken);
                    if (response.Result == false)
                    {
                        return Result.Failure<String>(
                            new Error("ResetPassword.Handler", $"Invalid token. Token: {decodedToken}"));
                    }
                }
                
                var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

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
        _ = app.MapPost("api/resetPassword/{token}", async (ResetPasswordModel model, ISender sender, string token) =>
        {
            var command = new ResetPassword.Command
            {
                Token = token,
                NewPassword = model.newPassword,
                Username = model.Username
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