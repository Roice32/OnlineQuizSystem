using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class GetCreatedQuizStatsResponse
    {
        public Guid UserID { get; set; }
        public Guid QuizId { get; set; }
        public List<QuizResultHeader> QuizResultHeaders { get; set; }
    }
}
