using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults
{
    public class ReviewNeededQuestionResult: QuestionResultBase
    {
        public string ReviewNeededAnswer { get; set; }
        public AnswerResult ReviewNeededResult { get; set; }

        public ReviewNeededQuestionResult(Guid userId, Guid questionId, float score, string reviewNeeded, AnswerResult reviewNeededResult):
            base(userId, questionId, score)
        {
            ReviewNeededAnswer = reviewNeeded;
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
