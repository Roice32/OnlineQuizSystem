namespace OQS.CoreWebAPI.Contracts;

public class CreateActiveQuizRequest
{
    public   Guid quizId { get; set; }
    public   Guid takenBy { get; set; }
    
}