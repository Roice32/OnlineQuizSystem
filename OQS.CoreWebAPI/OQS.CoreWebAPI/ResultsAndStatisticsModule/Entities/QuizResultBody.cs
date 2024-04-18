
namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class QuizResultBody
    {
        public Guid QuizzId { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<QuestionResult> QuestionResults { get; internal set; }
        private QuestionResult QuestionResult { get; set; } = new QuestionResult();
        public void ReviewAnswer(int finalScore)
        {
            QuestionResult.UpdateScore(finalScore);
        }
    }
}