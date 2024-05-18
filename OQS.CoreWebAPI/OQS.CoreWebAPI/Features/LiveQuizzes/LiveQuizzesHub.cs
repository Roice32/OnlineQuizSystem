using Microsoft.AspNetCore.SignalR;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Features.LiveQuizzes;

public class LiveQuizzesHub: Hub
{
    // public async Task Test()
    // {
    //     await Groups.AddToGroupAsync(Context.ConnectionId, "test");
    //     await Clients.All.SendAsync("Test","admin","Test message");
    // }
    
    public async Task JoinQuiz(ConnectionRequest connection)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, connection.Code);
        await Clients.Group(connection.Code).SendAsync("UserJoined","admin",$"User {connection.UserId} joined");
    }
}