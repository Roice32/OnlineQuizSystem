namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics
{
    public class QuizResultHeader
    {
        public Guid ResultId { get; set; }
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SubmittedAtUtc { get; set; }
        public float Score { get; set; }
        public bool ReviewPending { get; set; }

        public QuizResultHeader(Guid resultId, Guid quizId, Guid userId)
        {
            ResultId = resultId;
            QuizId = quizId;
            UserId = userId;
            SubmittedAtUtc = DateTime.UtcNow;
        }
    }
}
