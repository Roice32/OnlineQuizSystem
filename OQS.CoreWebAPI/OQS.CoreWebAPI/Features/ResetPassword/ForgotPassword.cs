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

namespace OQS.CoreWebAPI.Features.ResetPassword
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
                RuleFor(x => x.Email).NotEmpty();
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
                string resetLink = $"http://localhost:3000/reset_password/{encodedToken}";

                var body = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <title> Password Reset for Quiz Application</title>\r\n    <style>\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n            padding: 0;\r\n        }\r\n        h1{\r\n            text-align: center;\r\n            color: #1c4e4f;\r\n            font-size: 7svh;\r\n            text-shadow: 1px 1px 2px #0a2d2e;\r\n            padding-bottom: 20px;\r\n        }\r\n        .container {\r\n            width: 100%;\r\n            max-width: 700px;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            background-color: #deae9f;\r\n        }\r\n        p{\r\n            font-size: 23px;\r\n            color: #0a2d2e;\r\n        }\r\n        .reset-link {\r\n            display: inline-block;\r\n            font-size: 20px;\r\n            width: 100%;\r\n            height: 60px;\r\n            text-align: center;\r\n            line-height: 60px;\r\n            margin-top: 10px;\r\n            max-width: 700px;\r\n            text-decoration: none;\r\n            color: white;\r\n            background-color: #6a8e8f;\r\n\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <h1> Password Reset for Quiz Application</h1>\r\n        <p>Dear User,\r\n\r\n            We received a password reset request for your account associated with this email address for our Quiz Application.<br>\r\n            If you made this request, please click on the link below to reset your password:</p>\r\n        <a href=\"resetToken\" class=\"reset-link\">Reset Your Password</a>\r\n        <p>If you did not request a password reset, please ignore this email and no changes will be made to your account.<br>\r\n            Thank you for using our Quiz Application!<br><br>\r\n            Best regards,<br>\r\n            Quiz Application Team</p>\r\n    </div>\r\n</body>\r\n</html>\r\n";

                body = body.Replace("resetToken", resetLink);

                await emailService.SendEmailAsync(request.Email, "Instructions for Resetting Your Password", body);

                return Result.Success(Guid.Parse(user.Id));
            }
        }
    }

}
public class ForgotPasswordEndPoind : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("api/forgot_password", async (ForgotPasswordModel model, ISender sender) =>
        {
            var command = new OQS.CoreWebAPI.Features.ResetPassword.ForgotPassword.Command
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

