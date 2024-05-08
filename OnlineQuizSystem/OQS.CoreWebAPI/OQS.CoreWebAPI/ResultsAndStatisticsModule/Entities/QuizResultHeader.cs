namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizResultHeader
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set;  }
        public DateTime SubmittedAt { get; set; }
        public int CompletionTime { get; set; }
        public float Score { get; set; }
        public bool ReviewPending { get; set; }

        public QuizResultHeader(Guid quizId, Guid userId, int completionTime)
        {
            QuizId = quizId;
            UserId = userId;
            CompletionTime = completionTime;
            SubmittedAt = DateTime.Now;
        }
    }
}
