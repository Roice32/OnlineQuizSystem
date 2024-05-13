namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs
{
    public abstract class QuestionAnswerPairBase
    {
        public Guid QuestionId { get; set; }

        public QuestionAnswerPairBase(Guid questionId)
        {
            QuestionId = questionId;
        }
    }
}
