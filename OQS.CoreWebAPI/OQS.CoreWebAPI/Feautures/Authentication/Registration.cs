﻿using Carter;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Feautures.Authentication;
using OQS.CoreWebAPI.Shared;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Feautures.Authentication
{
    public class Registration
    {
        public record Command : IRequest<Result<Guid>>
        {
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Username).NotEmpty();
                RuleFor(x => x.FirstName).NotEmpty();
                RuleFor(x => x.LastName).NotEmpty();
                RuleFor(x => x.Email).NotEmpty().EmailAddress();
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly UserManager<User> userManager;
            private readonly RoleManager<IdentityRole> roleManager;
            private readonly IValidator<Command> validator;
            private readonly IEmailSender emailService;

            public Handler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IValidator<Command> validator, IEmailSender emailService)
            {
                this.userManager = userManager;
                this.roleManager = roleManager;
                this.validator = validator;
                this.emailService = emailService;
            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<Guid>(
                        new Error("Registration.Validator", validationResult.ToString()));
                }

                var userExists = await userManager.FindByNameAsync(request.Username!);
                if (userExists != null)
                {
                    return Result.Failure<Guid>(new Error("Registration", "User already exists"));
                }

                User user = new User()
                {
                    UserName = request.Username,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PasswordHash = request.Password,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var createUserResult = await userManager.CreateAsync(user, request.Password!);
                if (!createUserResult.Succeeded)
                {
                    return Result.Failure<Guid>(
                                               new Error("Registration", "User creation failed! Please check user details and try again."));
                }

                if (await roleManager.RoleExistsAsync(UserRole.User))
                {
                    await userManager.AddToRoleAsync(user, UserRole.User);
                }

                var body = "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <title>Welcome to Online Quiz Application</title>\r\n    <style>\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n            padding: 0;\r\n        }\r\n        h1{\r\n            text-align: center;\r\n            color: #1c4e4f;\r\n            font-size: 7svh;\r\n            text-shadow: 1px 1px 2px #0a2d2e;\r\n            padding-bottom: 20px;\r\n        }\r\n        .container {\r\n            width: 100%;\r\n            max-width: 700px;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            background-color: #deae9f;\r\n        }\r\n        p{\r\n            font-size: 23px;\r\n            color: #0a2d2e;\r\n        }\r\n        \r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <h1> Welcome to Online Quiz Application</h1>\r\n        <p>Dear usernameToBeReplaced, <br><br>\r\n            Thank you for registering with our Online Quiz Application! We're excited to have you in our community.\r\n            You can now start exploring hundreds of available quizzes, create your own quizzes, and share knowledge with other users.\r\n            If you have any questions or need assistance, don't hesitate to contact us at Online.Quiz@outlook.com.<br>\r\n            Welcome to our community! <br><br>\r\n\r\n            Best regards, <br>\r\n            Online Quiz Application Team</p>\r\n    </div>\r\n</body>\r\n</html>\r\n";
                    body = body.Replace("usernameToBeReplaced", request.Username);
                await emailService.SendEmailAsync(request.Email, "Account Confirmation & Welcome to Online Quiz Application",body);

                return Result.Success(Guid.Parse(user.Id));
            }
        }

    }
}
public class RegistrationEndPoind : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("api/registration", async (RegistrationModel model, ISender sender) =>
        {
            var command = new Registration.Command
            {
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password
            };


            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                return Results.Ok(new { message = "User created successfully!" });
            }
            else
            {
                return Results.Ok(new { message = result.Error.Message });
            }
        });
    }
}