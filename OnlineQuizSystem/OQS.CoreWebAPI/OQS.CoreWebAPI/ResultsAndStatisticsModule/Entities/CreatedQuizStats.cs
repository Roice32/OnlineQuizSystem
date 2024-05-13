namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities
{
    public class CreatedQuizStats
    {
        public Guid UserID { get; set; }
        public Guid QuizId { get; set; }
        public List<QuizResultHeader> QuizResultHeaders { get; set; }

        public CreatedQuizStats(Guid userID, Guid quizId, List<QuizResultHeader> quizResultHeaders)
        {
            UserID = userID;
            QuizId = quizId;
            QuizResultHeaders = quizResultHeaders;
        }
    }
}
