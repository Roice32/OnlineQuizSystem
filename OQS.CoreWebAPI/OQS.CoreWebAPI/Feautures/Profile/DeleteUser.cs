using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Feautures.Profile
{
    public class DeleteUser
    {
        public record Command : IRequest<Result<Result>>
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, Result<Result>>
        {
            private readonly UserManager<User> userManager;
            private readonly IValidator<Command> validator;

            public Handler(UserManager<User> userManager, IValidator<Command> validator)
            {
                this.userManager = userManager;
                this.validator = validator;
            }

            public async Task<Result<Result>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = validator.Validate(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<Result>(
                                               new Error("DeleteUser.Validator", validationResult.ToString()));
                }

                var user = await userManager.FindByIdAsync(request.Id.ToString());
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
        app.MapDelete("api/profile/{id}/delete_account", async (Guid id, ISender sender) =>
        {
            var command = new OQS.CoreWebAPI.Feautures.Profile.DeleteUser.Command
            {
                Id = id
            };
            var result = await sender.Send(command);
            if (result.IsFailure)
            {
                return Results.NotFound(result.Error);
            }
            return Results.Ok(result.Value);
        });
    }
}