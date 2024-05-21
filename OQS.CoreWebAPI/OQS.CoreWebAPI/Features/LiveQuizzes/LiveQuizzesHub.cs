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
        if (result.IsSuccess)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.Code);
            var liveQuiz = await _context.LiveQuizzes
                .Include(x => x.CreatedBy)
                .FirstOrDefaultAsync(x => x.Code == conn.Code);
            var adminId = await liveQuiz.getAdminConnectionId();
            var user = await _context.Users.FindAsync(conn.UserId);
            await Clients.Client(adminId).SendAsync("UserJoined", user.Name);
            await Clients.Caller.SendAsync("Joined",command.ConnectionId==adminId);
        }
        else
        {
            await Clients.Caller.SendAsync("Error", result.Error);
        }
    }

    public async Task StartQuiz()
    {
        var command = new StartLiveQuiz.StartQuizCommand(Context.ConnectionId);
        var result = await _sender.Send(command);
    }
   
}