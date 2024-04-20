namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults
{
    public class ChoiceQuestionResult: QuestionResultBase
    {
        public Dictionary<string, AnswerResult> ChoicesResults { get; set; }
        public ChoiceQuestionResult(Guid userId, Guid questionId, float score, Dictionary<string, AnswerResult> choicesResults):
            base(userId, questionId, score)
        {
            ChoicesResults = choicesResults;
        }
    }
}
