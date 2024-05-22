namespace OQS.CoreWebAPI.Entities;

public class  LiveQuizz
{
    public string Code { get; set; }
    public Quiz Quiz { get; set; }
    public User CreatedBy { get; set; }
    public List<UserConnection> Connections { get; set; } = new();
    
    public async Task<string> getAdminConnectionId()
    {
        if (Connections == null || CreatedBy == null)
        {
            return null;
        }
        var conn = Connections.FirstOrDefault(c => c.User != null && c.User.Id == CreatedBy.Id);
      
        return conn?.ConnectionId;
    }
}