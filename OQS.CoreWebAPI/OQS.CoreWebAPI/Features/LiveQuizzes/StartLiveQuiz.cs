using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.LiveQuizzes;

public class StartLiveQuiz
{
    public record StartQuizCommand(StartLiveQuizRequest request) : IRequest<Result<string>>;
    
    public class StartLiveQuizValidator : AbstractValidator<StartQuizCommand>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public StartLiveQuizValidator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            RuleFor(x => x.request.UserId)
                .MustAsync(UserExists)
                .WithMessage("Invalid User Id");

            RuleFor(x => x.request.QuizId)
                .MustAsync(LiveQuizExists)
                .WithMessage("Invalid Live Quiz Id");
        }

        private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            return await context.Users.FindAsync(userId) != null;
        }

        private async Task<bool> LiveQuizExists(Guid quizId, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            return await context.Quizzes.FindAsync(quizId) != null;
        }
    }
    public sealed class Handler : IRequestHandler<StartQuizCommand, Result<string>>
    {
        private readonly ApplicationDBContext _context;
        private readonly StartLiveQuizValidator _validator;
        private readonly IHubContext<LiveQuizzesHub> _hubContext;

        public Handler(ApplicationDBContext context, StartLiveQuizValidator validator, IHubContext<LiveQuizzesHub> hubContext)
        {
            _context = context;
            _validator = validator;
            _hubContext = hubContext;
        }

        public async Task<Result<string>> Handle(StartQuizCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<string>(new Error("StartQuiz.BadRequest", validationResult.ToString()));
            }

            var liveQuiz = await _context.LiveQuizzes.FindAsync(request.request.QuizId);
            var adminConnectionId = await liveQuiz.getAdminConnectionId();
            if (request.request.UserId.ToString() == adminConnectionId)
            {
                await _hubContext.Clients.Group(liveQuiz.Code).SendAsync("StartQuiz", request.request.QuizId);
            }
            else
            {
                return Result.Failure<string>(new Error("StartQuiz.Unauthorized", "Only the admin can start the quiz"));
            }

            return Result.Success<string>($"Quiz {request.request.QuizId} started by {request.request.UserId}");
        }
    }
}