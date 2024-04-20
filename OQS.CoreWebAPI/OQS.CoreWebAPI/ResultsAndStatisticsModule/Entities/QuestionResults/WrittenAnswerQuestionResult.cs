namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.ResultTypes
{
    public class WrittenAnswerQuestionResult:QuestionResultBase
    {
        public string WrittenAnswer { get; set; }
        
        public AnswerResult WrittenAnswerResult { get; set; }
        public WrittenAnswerQuestionResult(Guid userId, Guid questionId,string writtenAnswer,AnswerResult writtenAnswerResult ) : base(userId, questionId)
        {
            WrittenAnswer = writtenAnswer;
            WrittenAnswerResult = writtenAnswerResult;
        }
    }
   
}
