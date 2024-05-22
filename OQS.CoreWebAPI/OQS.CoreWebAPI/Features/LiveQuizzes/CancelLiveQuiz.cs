using MediatR;
using FluentValidation;
using OQS.CoreWebAPI.Shared;
using Microsoft.AspNetCore.SignalR;
using OQS.CoreWebAPI.Database;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Features.LiveQuizzes;

public class CancelLiveQuiz
{
    public record Command(string ConnectionId) : IRequest<Result>;

    public class Validator : AbstractValidator<Command>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Validator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            RuleFor(x => x.ConnectionId)
                .NotEmpty().WithMessage("ConnectionId is required.")
                .MustAsync(ConnectionExists).WithMessage("Invalid ConnectionId.");
        }

        private async Task<bool> ConnectionExists(string connectionId, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            return await context.UserConnections.AnyAsync(c => c.ConnectionId == connectionId, cancellationToken);
        }
    }

    public class Handler : IRequestHandler<Command, Result>
    {
        private readonly ApplicationDBContext _context;
        private readonly IHubContext<LiveQuizzesHub> _hubContext;
        private readonly IValidator<Command> _validator;
        
        public Handler(ApplicationDBContext context, IHubContext<LiveQuizzesHub> hubContext, IValidator<Command> validator)
        {
            _context = context;
            _hubContext = hubContext;
            _validator = validator;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            if (_context == null || _hubContext == null || _validator == null)
            {
                throw new ArgumentNullException(nameof(_context), "The context, hubContext or validator cannot be null.");
            }

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure(new Error("CancelLiveQuiz.ValidationFailed", validationResult.ToString()));
            }

            var liveQuiz = await GetLiveQuiz(request, cancellationToken);
            if (liveQuiz == null)
            {
                return Result.Failure(new Error("CancelLiveQuiz.NotFound", "Live quiz not found"));
            }

            var adminConnection = await liveQuiz.getAdminConnectionId();
            if (adminConnection == null || adminConnection != request.ConnectionId)
            {
                return Result.Failure(new Error("CancelLiveQuiz.Unauthorized", "User is not the admin"));
            }

            await NotifyClients(liveQuiz.Code);

            await RemoveLiveQuiz(liveQuiz, cancellationToken);

            return Result.Success();
        }

        private async Task<LiveQuizz> GetLiveQuiz(Command request, CancellationToken cancellationToken)
        {
            return await _context.LiveQuizzes
                .Include(lq => lq.Connections)
                .Include(lq => lq.CreatedBy)
                .FirstOrDefaultAsync(lq => lq.Connections.Any(c => c.ConnectionId == request.ConnectionId), cancellationToken);
        }

        private async Task NotifyClients(string groupCode)
        {
            if (_hubContext.Clients == null || _hubContext.Clients.Group == null)
            {
                throw new ArgumentNullException(nameof(_hubContext.Clients), "The Clients or Group property of hubContext cannot be null.");
            }

            await _hubContext.Clients.Group(groupCode).SendAsync("LiveQuizCanceled", "The live quiz has been canceled by the admin");
        }

        private async Task RemoveLiveQuiz(LiveQuizz liveQuiz, CancellationToken cancellationToken)
        {
            _context.LiveQuizzes.Remove(liveQuiz);
            await _context.SaveChangesAsync(cancellationToken);
        }

    }
}
