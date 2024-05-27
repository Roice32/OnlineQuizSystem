namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionAnswerPairs
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
