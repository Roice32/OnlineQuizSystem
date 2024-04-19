namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionAnswerPairClasses
{
    public class MultipleChoiceQAPair : QuestionAnswerPairBase
    {
        public List<string> MultipleChoiceAnswers { get; set; }

        public MultipleChoiceQAPair(Guid questionId, List<string> answers) : base(questionId)
        {
            MultipleChoiceAnswers = answers;
        }
    }
}
