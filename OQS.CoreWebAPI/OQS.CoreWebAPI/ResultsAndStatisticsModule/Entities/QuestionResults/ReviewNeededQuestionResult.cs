using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults
{
    public class ReviewNeededQuestionResult: QuestionResultBase
    {
        public string ReviewNeededAnswer { get; set; }
        public AnswerResult ReviewNeededResult { get; set; }

        public ReviewNeededQuestionResult(Guid userId, Guid questionId, float score, string reviewNeededAnswer, AnswerResult reviewNeededResult):
            base(userId, questionId, score)
        {
            ReviewNeededAnswer = reviewNeededAnswer;
            ReviewNeededResult = reviewNeededResult;
        }
    }
}
