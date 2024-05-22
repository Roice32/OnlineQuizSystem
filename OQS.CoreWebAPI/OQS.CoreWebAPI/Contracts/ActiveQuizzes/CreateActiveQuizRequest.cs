namespace OQS.CoreWebAPI.Contracts;

public class CreateActiveQuizRequest
{
    public   Guid QuizId { get; set; }
    public   Guid TakenBy { get; set; }
    
}