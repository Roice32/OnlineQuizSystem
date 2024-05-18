using System.Diagnostics;
using OQS.CoreWebAPI.Database;
using OQS.CoreWebAPI.Entities;

namespace OQS.CoreWebAPI.Features.WebSocket;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class QuizRoomHub : Hub
{
    private readonly ApplicationDBContext _context ;
    
    public async Task JoinRoom(UserConnection conn)
    {
      // Create a new Connection entity
    var connection = new UserConnection
    { 
        User = await _context.Users.FindAsync(conn.User),
        ConnectionId = Context.ConnectionId
    };
        // Save the Connection entity in the database
        //_context.Connections.Add(connection);
        //await _context.SaveChangesAsync();
        
        // Find the LiveQuiz entity in the database
        var liveQuiz = await _context.LiveQuizzes.FindAsync(conn.ConnectionId);

        // Add the connection to the LiveQuiz's connections
        Debug.Assert(liveQuiz != null, nameof(liveQuiz) + " != null");
        liveQuiz.Connections.Add(connection);

        // Save changes in the database
        await _context.SaveChangesAsync();

        // Add the user to the group
        await Groups.AddToGroupAsync(Context.ConnectionId, conn.ConnectionId);

        // Notify the group
        await Clients.Group(conn.ConnectionId)
            .SendAsync("ReceiveMessage", "admin", $"{conn.User} has joined the room");
    }
    
}