namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairs
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
