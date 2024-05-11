using Carter;
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

            //    await emailService.SendEmailAsync(request.Email, "Welcome to OQS", "Welcome to OQS");

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