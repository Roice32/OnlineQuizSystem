namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class FetchQuizResultHeaderResponse
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SubmittedAtUtc { get; set; }
        public int CompletionTime { get; set; }
        public float Score { get; set; }
        public bool ReviewPending { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string QuizName { get; set; } = string.Empty;
    }
}