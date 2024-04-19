using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.ResultTypes
{
    public class ReviewNeededQuestionResult: QuestionResultBase
    {
        public AnswerResult ReviewNeededResult { get; set; }

        public string ReviewNeeded { get; set; }
        public ReviewNeededQuestionResult(Guid userId, Guid questionId,string reviewNeeded, AnswerResult reviewNeededResult) : base(userId, questionId)
        {
            ReviewNeeded = reviewNeeded;
            ReviewNeededResult = reviewNeededResult;
        }

        public void UpdateScore(int finalScore)
        {
            Score = finalScore;
            if (Score == 0)
            {
                ReviewNeededResult = AnswerResult.Wrong;
                return;
            }
            // PLACEHOLDER
            QuestionBase questionFromDd = null;
            if (Score == questionFromDd.AllocatedPoints)
            {
                ReviewNeededResult = AnswerResult.Correct;
            }
            else
            {
                ReviewNeededResult = AnswerResult.PartiallyCorrect;
            }
        }
    }
}
