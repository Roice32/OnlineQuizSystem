namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics
{
    public class QuizResultHeader
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SubmittedAtUtc { get; set; }
        public int CompletionTime { get; set; }
        public float Score { get; set; }
        public bool ReviewPending { get; set; }

        public QuizResultHeader(Guid quizId, Guid userId, int completionTime)
        {
            QuizId = quizId;
            UserId = userId;
            CompletionTime = completionTime;
            SubmittedAtUtc = DateTime.UtcNow;
        }
    }
}
