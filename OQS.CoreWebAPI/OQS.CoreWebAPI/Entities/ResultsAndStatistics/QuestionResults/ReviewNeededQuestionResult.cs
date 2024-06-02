namespace OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults
{
    public class ReviewNeededQuestionResult : QuestionResultBase
    {
        public string ReviewNeededAnswer { get; set; }
        public string LLMReview { get; set; } = string.Empty;
        public AnswerResult ReviewNeededResult { get; set; }

        public ReviewNeededQuestionResult(Guid userId, Guid questionId, float score, string reviewNeededAnswer, AnswerResult reviewNeededResult) :
            base(userId, questionId, score)
        {
            ReviewNeededAnswer = reviewNeededAnswer;
            ReviewNeededResult = reviewNeededResult;
        }
    }
}
