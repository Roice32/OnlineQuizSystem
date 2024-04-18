using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuestionResult
    {
        public Guid QuestionId { get; set; }
        public List<object> SubmittedAnswers { get; set; } = new();
        public List<AnswerResult> AnswersTypes { get; set; } = new();
        public int Score { get; set; }

        public QuestionResult(Guid questionId, List<object> submittedAnswers)
        {
            QuestionId = questionId;
            SubmittedAnswers.AddRange(submittedAnswers);
        }
        public void UpdateScore(int finalScore)
        {
            Score = finalScore;
            if (Score == 0)
            {
                AnswersTypes[0] = AnswerResult.Wrong;
                return;
            }
            // PLACEHOLDER
            QuestionBase questionFromDd = null;
            if (Score == questionFromDd.AllocatedPoints)
            {
                AnswersTypes[0] = AnswerResult.Correct;
            }
            else
            {
                AnswersTypes[0] = AnswerResult.PartiallyCorrect;
            }
        }
    }
}
