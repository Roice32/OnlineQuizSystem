namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class GetQuizResultHeaderResponse
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int CompletionTime { get; set; }
        public float Score { get; set; }
        public bool ReviewPending { get; set; }
        public string UserName { get; set; }
        public string QuizName { get; set; }
    }
}
