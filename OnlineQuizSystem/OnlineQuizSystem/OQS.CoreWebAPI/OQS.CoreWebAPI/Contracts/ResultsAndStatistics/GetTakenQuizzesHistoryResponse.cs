using OQS.CoreWebAPI.Entities.ResultsAndStatistics;

namespace OQS.CoreWebAPI.Contracts.ResultsAndStatistics
{
    public class GetTakenQuizzesHistoryResponse
    {
        public Dictionary<Guid, string> QuizNames { get; set; }
        public List<QuizResultHeader> QuizResultHeaders { get; set; }
    }
}