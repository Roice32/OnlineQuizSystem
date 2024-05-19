using FluentValidation;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Shared;
using OQS.CoreWebAPI.Contracts;

namespace OQS.CoreWebAPI.Features.LiveQuizzes
{
    
        public record ConnectionRequest(Guid UserId, string Code) : IRequest<Result<bool>>;
    
    public class CreateJoinRoom
    {
        public class JoinRoomValidator : AbstractValidator<OQS.CoreWebAPI.Contracts.LiveQuizzes.ConnectionRequest>
        {

            private readonly IServiceScopeFactory _serviceScopeFactory;

            public JoinRoomValidator(IServiceScopeFactory serviceScopeFactory)
            { _serviceScopeFactory = serviceScopeFactory;
                RuleFor(x => x.UserId)
                    .MustAsync(UserExists)
                    .WithMessage("Invalid User Id");

                RuleFor(x => x.Code)
                    .MustAsync(LiveQuizExists)
                    .WithMessage("Invalid Live Quiz Code");
            }

            private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                return await context.Users.FindAsync(userId, cancellationToken) != null;
            }

            private async Task<bool> LiveQuizExists(string code, CancellationToken cancellationToken)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                return await context.LiveQuizzes.FindAsync(code) != null;
            }
        }

        public sealed class  Handler : IRequestHandler<OQS.CoreWebAPI.Contracts.LiveQuizzes.ConnectionRequest, Result<bool>>
        {
            private readonly ApplicationDBContext _context;
            private readonly JoinRoomValidator _validator;

            public Handler(ApplicationDBContext context, JoinRoomValidator validator)
            {
                _context = context;
                _validator = validator;
            }

            public async Task<Result<bool>> Handle(OQS.CoreWebAPI.Contracts.LiveQuizzes.ConnectionRequest request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<bool>(
                        new Error("JoinRoom.BadRequest",
                            validationResult.ToString()));
                }
                else
                {
                    return Result.Success<bool>(true);
                }
            }

        }


    }
}