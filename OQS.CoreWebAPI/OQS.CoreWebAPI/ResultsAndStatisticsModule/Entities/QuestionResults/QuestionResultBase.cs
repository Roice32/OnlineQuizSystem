using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.ResultTypes
{
    public abstract class QuestionResultBase
    {
        public Guid UserId { get; set; }
        public Guid QuestionId { get; set; }
        public float Score { get; set; }

        protected QuestionResultBase(Guid userId, Guid questionId, float score)
        {
            UserId = userId;
            QuestionId = questionId;
            Score = score;
        }
    }
}
