using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Features.LiveQuizzes;

public class DisconnectClient
{
    public record DisconnectClientCommand(string ConnectionId) : IRequest<Result>;
    public class Validator: AbstractValidator<DisconnectClientCommand>
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
    
    public class Handler: IRequestHandler<DisconnectClientCommand, Result>
    {
        private readonly ApplicationDBContext _context;
        private readonly IValidator<DisconnectClientCommand> _validator;
        private readonly IHubContext<LiveQuizzesHub> _hubContext;
        private readonly ISender _sender;
        public Handler(ApplicationDBContext context, IValidator<DisconnectClientCommand> validator, IHubContext<LiveQuizzesHub> hubContext, ISender sender)
        {
            _context = context;
            _validator = validator;
            _hubContext = hubContext;
            _sender = sender;
        }
       
        public async Task<Result> Handle(DisconnectClientCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure(new Error("DisconnectClient.ValidationFailed", validationResult.ToString()));
            }
            var connection = await _context.UserConnections
                .Include(con=>con.LiveQuizz)
                .Include(conn=>conn.User)
                .FirstAsync(conn=>conn.ConnectionId == request.ConnectionId);
            
                var liveQuiz = await _context.LiveQuizzes.Include(q => q.Connections).Include(q=>q.CreatedBy).FirstOrDefaultAsync(q=>q.Code == connection.LiveQuizz.Code);
                if (liveQuiz == null)
                {
                    _context.UserConnections.Remove(connection);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var adminConnectionId = await liveQuiz.getAdminConnectionId();
                    if (adminConnectionId == request.ConnectionId)
                    {
                        var command = new CancelLiveQuiz.Command(request.ConnectionId);
                        var result = await _sender.Send(command);
                    }
                    else
                    {
                        await _hubContext.Groups.RemoveFromGroupAsync(request.ConnectionId, connection.LiveQuizz.Code);
                        await _hubContext.Clients.Client(adminConnectionId).SendAsync("UserLeft", connection.User.UserName);
                        _context.UserConnections.Remove(connection);
                        await _context.SaveChangesAsync();
                    }
                }

                return Result.Success();

        }
    }
   
}