namespace OQS.CoreWebAPI.Entities;

public class UserConnection
{
    public String ConnectionId { get; set; }
    public User? User { get; set; }
    
    public Guid UserId { get; set; }
    
    public LiveQuizz LiveQuizz { get; set; }
}