using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using OQS.CoreWebAPI.Contracts.LiveQuizzes;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Features.LiveQuizzes;

public class LiveQuizzesHub: Hub
{
    private readonly ApplicationDBContext _context;
    private readonly Handler _handler;
    public LiveQuizzesHub(ApplicationDBContext context,Handler handler)
    {
        _context = context;
        _handler = handler;
    }
    
    public async Task JoinRoom(ConnectionRequest conn)
    {
         //validator verifica User si Code
        
        //daca nu e valid trimit mesaj celui care a facut conexiunea
        // trimit o metoda clientilor "ConnectionDenied" 
        
        
        /*var validator = new JoinRoomValidator(_context);
        var validationResult = await validator.ValidateAsync(conn);
        
        if (!validationResult.IsValid)
        {
            await Clients.Caller.SendAsync("ConnectionDenied", validationResult.ToString());
            return;
        }*/
        
        var validationResult = await _handler.Handle(conn, CancellationToken.None);
        
        if (validationResult.IsFailure)
        {
            await Clients.Caller.SendAsync("ConnectionDenied", validationResult.Error);
            return;
        }
        
        var ConnectedUser = await _context.Users.FindAsync(conn.UserId);
         var connection = new UserConnection
         { 
             User = ConnectedUser,
             ConnectionId = Context.ConnectionId,
             UserId = ConnectedUser.Id
         };
         
        // Find the LiveQuiz entity in the database
        var liveQuiz = await _context.LiveQuizzes.FindAsync(conn.Code);

        // Add the connection to the LiveQuiz's connections
        liveQuiz.Connections.Add(connection);

        // Save changes in the database
        await _context.SaveChangesAsync();

        // Add the user to the group
        await Groups.AddToGroupAsync(Context.ConnectionId, conn.Code);

        // Notify the group
        await Clients.Group(conn.Code)
            .SendAsync("ReceiveMessage", $"{ConnectedUser.Name} has joined the room");
    }
}