namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults
{
    public class ChoiceQuestionResult : QuestionResultBase
    {
        public string PseudoDictionaryChoicesResults { get; set; }
        public ChoiceQuestionResult(Guid userId,
            Guid questionId,
            float score,
            string pseudoDictionaryChoicesResults) : base(userId, questionId, score)
        {
            PseudoDictionaryChoicesResults = pseudoDictionaryChoicesResults;
        }
    }
}