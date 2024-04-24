using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class GetTakenQuizzesHistoryResponse
    {
        public Dictionary<Guid, string> QuizNames { get; set; }
        public List<QuizResultHeader> QuizResultHeaders { get; set; }
    }
}