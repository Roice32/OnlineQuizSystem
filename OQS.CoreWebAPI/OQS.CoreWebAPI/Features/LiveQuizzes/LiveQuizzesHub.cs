using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
    public async Task CancelQuiz(CancelLiveQuizRequest request)
    {
        var command = new CancelLiveQuiz.Command(Context.ConnectionId);
        var result = await _sender.Send(command);
        if (result.IsSuccess)
        {
            await Clients.Group(request.Code).SendAsync("LiveQuizCanceled", "The live quiz has been canceled by the admin");
        }
        else
        {
            await Clients.Caller.SendAsync("Error", result.Error);
        }
    }
}