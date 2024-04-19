namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairClasses
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
