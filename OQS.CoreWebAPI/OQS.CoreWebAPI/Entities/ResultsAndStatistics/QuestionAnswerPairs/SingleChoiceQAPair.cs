namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionAnswerPairs
{
    public class SingleChoiceQAPair : QuestionAnswerPairBase
    {
        public string SingleChoiceAnswer { get; set; }

        public SingleChoiceQAPair(Guid questionId, string answer) : base(questionId)
        {
            SingleChoiceAnswer = answer;
        }
    }
}
