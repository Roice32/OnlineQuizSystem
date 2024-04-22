using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class GetCreatedQuizStatsResponse
    {
        public string QuizName { get; set; }
        public Dictionary<Guid, string> UsersNames { get; set; }
        public List<QuizResultHeader> QuizResultHeaders { get; set; }
    }
}
