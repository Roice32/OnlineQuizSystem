namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuestionResult
    {
        public Guid QuestionId { get; set; }
        public List<Object> SubmittedAnswers { get; set; }= new List<Object>();
        public List<AnswerResult> AnswerType { get; set; } = new List<AnswerResult>();
        public int Score { get; set; }

        public void UpdateScore(int finalScore)
        {
            finalScore += Score;
        }

    }
}
