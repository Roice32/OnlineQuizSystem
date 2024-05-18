using FluentValidation;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.LiveQuizzes
{
    public class JoinLiveQuiz
    {
        public record ConnectionCommand(Guid UserId, string Code, string ConnectionId) : IRequest<Result<string>>;


        public class JoinLiveQuizValidator : AbstractValidator<ConnectionCommand>
        {

            private readonly IServiceScopeFactory _serviceScopeFactory;

            public JoinLiveQuizValidator(IServiceScopeFactory serviceScopeFactory)
            {
                _serviceScopeFactory = serviceScopeFactory;

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
                return await context.Users.FindAsync(userId) != null;
            }

            private async Task<bool> LiveQuizExists(string code, CancellationToken cancellationToken)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                return await context.LiveQuizzes.FindAsync(code) != null;
            }
        }

        public sealed class Handler : IRequestHandler<ConnectionCommand, Result<string>>
        {
            private readonly ApplicationDBContext _context;
            private readonly JoinLiveQuizValidator _validator;

            public Handler(ApplicationDBContext context, JoinLiveQuizValidator validator)
            {
                _context = context;
                _validator = validator;
            }

            public async Task<Result<string>> Handle(ConnectionCommand request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Result.Failure<string>(new Error("JoinRoom.BadRequest", validationResult.ToString()));
                }

                var ConnectedUser = await _context.Users.FindAsync(request.UserId);
                var connection = new UserConnection
                {
                    User = ConnectedUser,
                    ConnectionId = request.ConnectionId,
                    UserId = ConnectedUser.Id
                };

                var liveQuiz = await _context.LiveQuizzes.FindAsync(request.Code);
                liveQuiz.Connections.Add(connection);
                await _context.SaveChangesAsync();


                return Result.Success<string>($"{ConnectedUser.Name} joined the quiz");

            }

        }


    }
}