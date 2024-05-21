using OQS.CoreWebAPI.Entities.ResultsAndStatistics;

namespace OQS.CoreWebAPI.Contracts.ResultsAndStatistics
{
    public class GetCreatedQuizStatsResponse
    {
        public string QuizName { get; set; } = string.Empty;
        public Dictionary<Guid, string> UserNames { get; set; } = new();
        public List<QuizResultHeader> QuizResultHeaders { get; set; } = new();
    }
}