namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.ResultTypes
{
    public abstract class ChoiceQuestionResult:QuestionResultBase
    {
        public Dictionary<string, AnswerResult> ChoicesResults { get; set; }
        public ChoiceQuestionResult(Guid userId, Guid questionId, float score, Dictionary<string, AnswerResult> choicesResults):
            base(userId, questionId, score)
        {
            ChoicesResults = choicesResults;
        }
    }
}
