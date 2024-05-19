using OQS.CoreWebAPI.Shared;

namespace OQS.CoreWebAPI.Entities;

public class LiveQuizz
{
    public string Code { get; set; }
    public Quiz Quiz { get; set; }
    public User CreatedBy { get; set; }
    public List<UserConnection> Connections { get; set; } = new();
    
    public async Task<string> getAdminConnectionId()
    {
        var conn= Connections.FirstOrDefault(c => c.User.Id == CreatedBy.Id);
      
        return conn.ConnectionId;
    }
}