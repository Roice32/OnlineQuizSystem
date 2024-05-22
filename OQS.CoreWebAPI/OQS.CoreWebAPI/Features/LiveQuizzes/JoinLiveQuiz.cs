using FluentValidation;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
            private readonly IHubContext<LiveQuizzesHub> _hubContext;

            public Handler(ApplicationDBContext context, JoinLiveQuizValidator validator, IHubContext<LiveQuizzesHub> hubContext)
            {
                _context = context;
                _validator = validator;
                _hubContext = hubContext;
            }
            

            public async Task<Result<string>> Handle(ConnectionCommand request, CancellationToken cancellationToken)
            {
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var error=new Error("JoinRoom.BadRequest", validationResult.ToString());
                    await _hubContext.Clients.Client(request.ConnectionId).SendAsync("Error", error);
                    return Result.Failure<string>(error);
                }

                var ConnectedUser = await _context.Users.FindAsync(request.UserId);
                var connection = new UserConnection
                {
                    User = ConnectedUser,
                    ConnectionId = request.ConnectionId,
                    UserId = ConnectedUser.Id
                };

                var liveQuiz = await _context.LiveQuizzes.Include(x => x.CreatedBy).Include(x=>x.Connections).FirstOrDefaultAsync(q=>q.Code==request.Code);
                liveQuiz.Connections.Add(connection);
                await _context.SaveChangesAsync();
                
                await _hubContext.Groups.AddToGroupAsync(request.ConnectionId, request.Code);
               
                var adminId = await liveQuiz.getAdminConnectionId();
                await _hubContext.Clients.Client(adminId).SendAsync("UserJoined", ConnectedUser.Name);
                await _hubContext.Clients.Client(request.ConnectionId).SendAsync("Joined",request.ConnectionId==adminId);
                return Result.Success<string>($"{ConnectedUser.Name} joined the quiz");

            }

        }


    }
}