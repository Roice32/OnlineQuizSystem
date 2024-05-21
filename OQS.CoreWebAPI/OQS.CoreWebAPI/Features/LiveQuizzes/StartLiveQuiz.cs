using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using OQS.CoreWebAPI.Entities.ActiveQuiz;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.LiveQuizzes;

public class StartLiveQuiz
{
    public record StartQuizCommand(string connectionID) : IRequest<Result<string>>;
    
    public class StartLiveQuizValidator : AbstractValidator<StartQuizCommand>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public StartLiveQuizValidator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            RuleFor(x => x.connectionID)
                .MustAsync(AdminConnectionIdMatches)
                .WithMessage("You are not authorized to start the quiz.");
        }
        private async Task<bool> AdminConnectionIdMatches(string connectionId, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var liveQuiz = await context.LiveQuizzes.Include(q => q.Connections)
                .FirstOrDefaultAsync(q => q.Connections.Any(c => c.ConnectionId == connectionId));
            if (liveQuiz == null)
            {
                return false;
            }
            var adminConnectionId = await liveQuiz.getAdminConnectionId();
            return connectionId == adminConnectionId;
        }
        
    }
    public sealed class Handler : IRequestHandler<StartQuizCommand, Result<string>>
    {
        private readonly ApplicationDBContext _context;
        private readonly StartLiveQuizValidator _validator;
        private readonly IHubContext<LiveQuizzesHub> _hubContext;
        private readonly ISender _sender;

        public Handler(ApplicationDBContext context, StartLiveQuizValidator validator, IHubContext<LiveQuizzesHub> hubContext, ISender sender)
        {
            _context = context;
            _validator = validator;
            _hubContext = hubContext;
            _sender = sender;
        }

        public async Task<Result<string>> Handle(StartQuizCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<string>(new Error("StartQuiz.BadRequest", validationResult.ToString()));
            }
            var connectionResult = await _context.UserConnections.Include(q => q.LiveQuizz)
                .FirstOrDefaultAsync(q => q.ConnectionId == request.connectionID);

            var liveQuiz =  connectionResult.LiveQuizz;
            foreach (var connection in liveQuiz.Connections)
            {
                var command = new CreateActiveQuiz.QuizCreation(liveQuiz.Quiz.Id, connection.User.Id);
                var result = await _sender.Send(command);
                await _hubContext.Clients.Client(connection.ConnectionId).SendAsync("StartQuiz", result);
            }
            return Result.Success("Quiz Started Successfully");
        }
    }
}