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

namespace OQS.CoreWebAPI
{ 
   // public record  OQS.CoreWebAPI.Contracts.LiveQuizzes.ConnectionRequest (Guid UserId, string Code) : IRequest<Result<bool>>;

    public class JoinRoomValidator : AbstractValidator<OQS.CoreWebAPI.Contracts.LiveQuizzes.ConnectionRequest>
    {
       
        private readonly ApplicationDBContext _context;

        public JoinRoomValidator(ApplicationDBContext context)
        {
            _context = context;

            RuleFor(x => x.UserId)
                .MustAsync(UserExists)
                .WithMessage("Invalid User Id");

            RuleFor(x => x.Code)
                .MustAsync(LiveQuizExists)
                .WithMessage("Invalid Live Quiz Code");
        }

        private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Users.FindAsync(userId) != null;
        }

        private async Task<bool> LiveQuizExists(string code, CancellationToken cancellationToken)
        {
            return await _context.LiveQuizzes.FindAsync(code) != null;
        }
    }

    public sealed class Handler : IRequestHandler<OQS.CoreWebAPI.Contracts.LiveQuizzes.ConnectionRequest, Result<bool>>
        {
            private readonly ApplicationDBContext _context;
            private readonly JoinRoomValidator _validator;
    
            public Handler(ApplicationDBContext context, JoinRoomValidator validator)
            {
                _context = context;
                _validator = validator;
            }
    
            public async Task<Result<bool>> Handle(ConnectionRequest request, CancellationToken cancellationToken)
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