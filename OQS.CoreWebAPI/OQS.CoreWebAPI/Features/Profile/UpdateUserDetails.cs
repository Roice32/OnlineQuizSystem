using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using OQS.CoreWebAPI.Contracts.Models;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Shared;
using System.Threading;
using System.Threading.Tasks;

namespace OQS.CoreWebAPI.Features.Profile
{
    public static class UpdateUserDetails
    {
        public class Command : IRequest<Result>
        {
            public string UserId { get; set; }
            public UpdateUserDetailsModel UpdateModel { get; set; }
        }

        internal sealed class Handler : IRequestHandler<Command, Result>
        {
            private readonly UserManager<User> _userManager;

            public Handler(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return Result.Failure(new Error("UpdateUserDetails.Handler", "User not found."));
                }

                user.UserName = request.UpdateModel.Username;
                user.FirstName = request.UpdateModel.FirstName;
                user.LastName = request.UpdateModel.LastName;
                user.Email = request.UpdateModel.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Result.Success();
                }

                return Result.Failure(new Error("UpdateUserDetails.Handler", "Failed to update user details."));
            }
        }

        public class UpdateUserDetailsModel
        {
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.UpdateModel.Username).NotEmpty().WithMessage("Username is required");
                RuleFor(x => x.UpdateModel.FirstName).NotEmpty().WithMessage("First name is required");
                RuleFor(x => x.UpdateModel.LastName).NotEmpty().WithMessage("Last name is required");
                RuleFor(x => x.UpdateModel.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required");
            }
        }
    }
}
