namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class TakenQuizzesHistory
    {
        public Guid UserId { get; set; }
        public List<QuizResultHeader> QuizResultHeaders { get; set; }

        public TakenQuizzesHistory(Guid userId, List<QuizResultHeader> quizResultHeaders)
        {
            UserId = userId;
            QuizResultHeaders = quizResultHeaders;
        }
    }
}
