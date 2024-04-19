namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.ResultTypes
{
    public abstract class ChoiceQuestionResult:QuestionResultBase
    {
        public Dictionary<String, AnswerResult> ChoicesResults { get; set; }
        public ChoiceQuestionResult(Guid userId, Guid questionId,Dictionary<String, AnswerResult> choicesResults ) : base(userId, questionId)
        {
            ChoicesResults = choicesResults;
        }
    }
}
