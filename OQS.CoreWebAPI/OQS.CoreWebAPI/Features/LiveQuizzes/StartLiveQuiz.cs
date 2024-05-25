using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
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
            var liveQuiz = await context.LiveQuizzes.Include(q => q.Connections).Include(q=>q.CreatedBy)
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
                var error=new Error("StartQuiz.BadRequest", validationResult.ToString());
                await _hubContext.Clients.Client(request.connectionID).SendAsync("Error", error);
                return Result.Failure<string>(error);
            }
            var connectionResult = await _context.UserConnections
                .Include(q => q.LiveQuizz)
                .Include(q=>q.LiveQuizz.Connections)
                .Include(q=>q.LiveQuizz.Connections)
                .Include(q=>q.LiveQuizz.Quiz)
                .FirstOrDefaultAsync(q => q.ConnectionId == request.connectionID);

            var liveQuiz =  connectionResult.LiveQuizz;
          
            List<Task> tasks = [];
            var connections= await _context.UserConnections
                .Include(q=>q.User)
                .Include(q=>q.LiveQuizz)
                .Where(q=>q.LiveQuizz.Code==liveQuiz.Code).ToListAsync();
            foreach (var connection in connections)
            {
                if(connection.ConnectionId!=request.connectionID)
                 tasks.Add(NotifyClients(connection));
            }
            await Task.WhenAll(tasks);
            await _hubContext.Clients.Client(request.connectionID).SendAsync("QuizStartedAdmin", Result.Success<Guid>(liveQuiz.Quiz.Id));
            return Result.Success("Quiz Started Successfully");
        }
        private async Task NotifyClients(UserConnection connection)
        {
            var result = await _sender.Send(new CreateActiveQuiz.QuizCreation(connection.LiveQuizz.Quiz.Id, connection.UserId));
            await _hubContext.Clients.Client(connection.ConnectionId).SendAsync("QuizStarted", Result.Success<Guid>(result.Value.Id));
        }
    }
    
   
}