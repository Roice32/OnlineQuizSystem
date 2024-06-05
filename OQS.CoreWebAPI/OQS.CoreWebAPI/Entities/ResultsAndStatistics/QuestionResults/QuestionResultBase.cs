namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults
{
    public abstract class QuestionResultBase
    {
        public Guid ResultId { get; set; }
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public float Score { get; set; }

        protected QuestionResultBase(Guid resultId, Guid userId, Guid questionId, float score)
        {
            ResultId = resultId;
            UserId = userId;
            QuestionId = questionId;
            Score = score;
        }

        protected QuestionResultBase(Guid userId, Guid questionId, float score)
        {
            UserId = userId;
            QuestionId = questionId;
            Score = score;
        }
    }
}
