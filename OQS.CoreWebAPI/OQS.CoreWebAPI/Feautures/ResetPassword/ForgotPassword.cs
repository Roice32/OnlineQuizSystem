using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Features.Authentication;
using OQS.CoreWebAPI.Features.Authentication.IEmailSender;
using OQS.CoreWebAPI.Shared;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

namespace OQS.CoreWebAPI.Features.Authentication
{
    public class ForgotPassword
    {
        public record Command : IRequest<Result>
        {
            public string Email { get; set; }
            public string Username { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
            }
        }

        public class Handler : IRequestHandler<Command, Result>
        {
            private readonly UserManager<User> userManager;
            private readonly IValidator<Command> validator;
            private readonly IConfiguration configuration;
            private readonly IEmailSender emailService;

            public Handler(UserManager<User> userManager, IValidator<Command> validator, IConfiguration configuration, IEmailSender emailService)
            {
                this.userManager = userManager;
                this.validator = validator;
                this.configuration = configuration;
                this.emailService = emailService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<String>(
                                               new Error("Authentication.Validator", validationResult.ToString()));
                }

                var user = await userManager.FindByNameAsync(request.Username!);
                if (user == null)
                {
                    return Result.Failure(new Error("ForgotPassword", "User not found"));
                }

                if (user.Email != request.Email)
                {
                    return Result.Failure(
                        new Error("ForgotPassword.Handler", "Invalid user or email."));
                }


                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                string encodedToken = HttpUtility.UrlEncode(token);
                string resetLink = $"http://localhost:3000/reset-password/{encodedToken}";

                await emailService.SendEmailAsync(request.Email, "Resetare parolă", $"Pentru a reseta parola, accesați link-ul următor: {resetLink}");

                return Result.Success(Guid.Parse(user.Id));
            }
        }
    }

}
public class ForgotPasswordEndPoind : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("api/forgot-password", async (ForgotPasswordModel model, ISender sender) =>
        {
            var command = new ForgotPassword.Command
            {
                Email = model.Email,
                Username = model.Username
            };

            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(new { message = "Email sent successfully!" });
            }
            else
            {
                return Results.Ok(new { message = result.Error.Message });
            }
        });
    }
}