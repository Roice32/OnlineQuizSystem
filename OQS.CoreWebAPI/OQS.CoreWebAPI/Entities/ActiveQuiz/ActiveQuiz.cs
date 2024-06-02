namespace OQS.CoreWebAPI.Entities.ActiveQuiz;

public class ActiveQuiz
{
    public Guid Id { get; set; }
    public   Quiz Quiz { get; set; }
    public   User User { get; set; }
    public   DateTime StartedAt { get; set; }
    
}   