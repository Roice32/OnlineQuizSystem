namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuestionResult
    {
        public Guid QuestionId { get; set; }
        public List<Object> SubmittedAnswers { get; set; } = new List<Object>();
        public List<AnswerResult> AnswerType { get; private set; } = new List<AnswerResult>();
        public int Score { get; private set; }

        public void UpdateScore(int finalScore)
        {
           Score+=finalScore;
        }

    }
}