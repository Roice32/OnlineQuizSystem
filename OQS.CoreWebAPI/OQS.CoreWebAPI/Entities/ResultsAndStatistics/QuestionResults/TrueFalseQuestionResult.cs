namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults
{
    public class TrueFalseQuestionResult : QuestionResultBase
    {
        public AnswerResult TrueFalseAnswerResult { get; set; }

        public TrueFalseQuestionResult(Guid resultId, Guid userId, Guid questionId, float score, AnswerResult trueFalseAnswerResult) :
            base(resultId, userId, questionId, score)
        {
            TrueFalseAnswerResult = trueFalseAnswerResult;
        }

        public TrueFalseQuestionResult(Guid userId, Guid questionId, float score, AnswerResult trueFalseAnswerResult) :
            base(userId, questionId, score)
        {
            TrueFalseAnswerResult = trueFalseAnswerResult;
        }
    }
}
