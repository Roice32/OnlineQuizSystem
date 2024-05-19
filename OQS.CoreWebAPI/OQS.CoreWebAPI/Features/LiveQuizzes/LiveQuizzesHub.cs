using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;

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
    
    public async Task JoinRoom(ConnectionRequest conn)
    {
        var command = new JoinLiveQuiz.ConnectionCommand(conn.UserId, conn.Code, Context.ConnectionId);
        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.Code);
            await Clients.Group(conn.Code).SendAsync("UserJoined", result.Value);
        }
        else
        {
            await Clients.Caller.SendAsync("Error", result.Error);
        }
    }
}