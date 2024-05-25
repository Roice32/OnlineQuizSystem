namespace OQS.CoreWebAPI.Contracts.LiveQuizzes;

public class CreateLiveQuizRequest
{
    public Guid QuizId { get; set; }
    public string UserId { get; set; }
}