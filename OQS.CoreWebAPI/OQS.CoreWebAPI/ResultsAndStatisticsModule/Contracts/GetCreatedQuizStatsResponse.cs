using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class GetCreatedQuizStatsResponse
    {
        public string QuizName { get; set; } = string.Empty;
        public Dictionary<Guid, string> UsersNames { get; set; } = new();
        public List<QuizResultHeader> QuizResultHeaders { get; set; } = new();
    }
}