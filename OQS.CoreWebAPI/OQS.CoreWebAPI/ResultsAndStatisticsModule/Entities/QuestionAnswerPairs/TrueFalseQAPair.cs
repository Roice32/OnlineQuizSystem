namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairClasses
{
    public class TrueFalseQAPair : QuestionAnswerPairBase
    {
        public bool TrueFalseAnswer { get; set; }

        public TrueFalseQAPair(Guid questionId, bool answer) : base(questionId)
        {
            TrueFalseAnswer = answer;
        }
    }
}
