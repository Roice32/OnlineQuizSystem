namespace OQS.CoreWebAPI.Contracts.LiveQuizzes;

public class CancelLiveQuizRequest
{
    public string ConnectionId { get; set; }
    public string Code { get; set; }
}