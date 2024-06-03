namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionAnswerPairs
{
    public class WrittenQAPair : QuestionAnswerPairBase
    {
        public string WrittenAnswer { get; set; }

        public WrittenQAPair(Guid questionId, string answer) : base(questionId)
        {
            WrittenAnswer = answer;
        }
    }
}
