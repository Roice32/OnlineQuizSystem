namespace OQS.CoreWebAPI.Contracts.LiveQuizzes;

public class ConnectionRequest 
{
    public Guid UserId { get; set; }
    public string Code { get; set; }
}