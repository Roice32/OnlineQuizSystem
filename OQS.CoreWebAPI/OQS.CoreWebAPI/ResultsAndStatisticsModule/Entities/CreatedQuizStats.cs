namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class CreatedQuizStats
    {
        public Guid UserID { get; set; }
        public Guid QuizId { get; set; }
        public List<QuizResultHeader> QuizResultHeaders { get; set; }

        public CreatedQuizStats(Guid userID, Guid quizId)
        {
            UserID = userID;
            QuizId = quizId;
            QuizResultHeaders = FetchQuizResultHeaders();
        }

        private List<QuizResultHeader> FetchQuizResultHeaders()
        {
            // Database fetching logic here
            // PLACEHOLDER
            return new List<QuizResultHeader>();
        }
    }
}
