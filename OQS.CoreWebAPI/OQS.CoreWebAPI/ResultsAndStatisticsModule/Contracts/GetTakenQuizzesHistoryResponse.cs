using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class GetTakenQuizzesHistoryResponse
    {
        public Dictionary<Guid, string> QuizNames { get; set; } = new();
        public List<QuizResultHeader> QuizResultHeaders { get; set; } = new();
    }
}