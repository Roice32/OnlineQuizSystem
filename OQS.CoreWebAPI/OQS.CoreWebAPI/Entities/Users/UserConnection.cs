namespace OQS.CoreWebAPI.Entities;

public class UserConnection
{
    public User User { get; set; } = new User();
    public string QuizRoomId { get; set; } = string.Empty;
    
}