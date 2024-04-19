
namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.ResultTypes
{
    public class TrueFalseQuestionResult : QuestionResultBase
    {
        public bool TrueFalsePickedAnswer { get; set; }
        public AnswerResult TrueFalseAnswerResult { get; set; }

        public TrueFalseQuestionResult(Guid userId, Guid questionId, bool trueFalsePickedAnswer, AnswerResult trueFalseAnswerResult) : base(userId, questionId)
        {
            TrueFalsePickedAnswer = trueFalsePickedAnswer;
            TrueFalseAnswerResult = trueFalseAnswerResult;
        }
    }
}
