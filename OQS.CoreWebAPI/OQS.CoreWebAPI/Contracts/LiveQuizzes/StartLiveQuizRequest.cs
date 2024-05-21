namespace OQS.CoreWebAPI.Contracts.LiveQuizzes;

public class StartLiveQuizRequest
{
    public Guid QuizId { get; set; }
    public Guid UserId { get; set; }
}