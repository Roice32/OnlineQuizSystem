namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults
{
    public class WrittenAnswerQuestionResult:QuestionResultBase
    {
        public string WrittenAnswer { get; set; }
        
        public AnswerResult WrittenAnswerResult { get; set; }
        public WrittenAnswerQuestionResult(Guid userId, Guid questionId, float score, string writtenAnswer,AnswerResult writtenAnswerResult ):
            base(userId, questionId, score)
        {
            WrittenAnswer = writtenAnswer;
            WrittenAnswerResult = writtenAnswerResult;
        }
    }
   
}
