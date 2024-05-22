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
   
}