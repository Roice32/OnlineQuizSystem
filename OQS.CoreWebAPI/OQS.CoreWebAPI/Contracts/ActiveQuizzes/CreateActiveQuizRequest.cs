namespace OQS.CoreWebAPI.Contracts;

public class CreateActiveQuizRequest
{
    public   Guid QuizId { get; set; }
    public   string TakenBy { get; set; }
    
}