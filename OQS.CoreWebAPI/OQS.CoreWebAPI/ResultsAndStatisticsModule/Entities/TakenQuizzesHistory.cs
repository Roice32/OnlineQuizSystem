namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class TakenQuizzesHistory
    {
        public Guid UserId { get; set; }
        public List<QuizResultHeader> QuizResultHeaders { get; set; }

        public TakenQuizzesHistory(Guid userId)
        {
            UserId = userId;
            QuizResultHeaders = FetchQuizResultHeaders(UserId);
        }

        private List<QuizResultHeader> FetchQuizResultHeaders(Guid userId)
        {
            // Database fetching logic here
            // PLACEHOLDER
            return new List<QuizResultHeader>();
        }
    }
}
