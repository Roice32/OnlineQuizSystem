namespace OQS.CoreWebAPI.Contracts.LiveQuizzes;

public class CreateLiveQuizRequest
{
    public Guid QuizId { get; set; }
    public Guid UserId { get; set; }
}