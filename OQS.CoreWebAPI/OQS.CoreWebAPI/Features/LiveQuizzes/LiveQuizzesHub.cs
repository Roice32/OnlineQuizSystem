
using MediatR;
using Microsoft.AspNetCore.SignalR;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Database;


namespace OQS.CoreWebAPI.Features.LiveQuizzes;

public class LiveQuizzesHub: Hub
{
    private readonly ApplicationDBContext _context;
    private readonly ISender _sender;
    public LiveQuizzesHub(ApplicationDBContext context,ISender sender)
    {
        _context = context;
        _sender = sender;
    }
    
    public async Task JoinQuiz(ConnectionRequest conn)
    {
        var command = new JoinLiveQuiz.ConnectionCommand(conn.UserId, conn.Code, Context.ConnectionId);
        var result = await _sender.Send(command);
    }

    public async Task StartQuiz()
    {
        var command = new StartLiveQuiz.StartQuizCommand(Context.ConnectionId);
        var result = await _sender.Send(command);
    }
    public async Task CancelQuiz()
    {
        var command = new CancelLiveQuiz.Command(Context.ConnectionId);
        var result = await _sender.Send(command);
        
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var command = new DisconnectClient.DisconnectClientCommand(Context.ConnectionId);
        var result = await _sender.Send(command);
        base.OnDisconnectedAsync(exception);
    }
    
    
}