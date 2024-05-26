using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Temp;

namespace OQS.CoreWebAPI.Contracts.ResultsAndStatistics
{
    public class FetchQuizResultBodyResponse
    {
        public List<QuestionResponse> Questions { get; set; } = new();
        public List<QuestionResultResponse> QuestionResults { get; set; } = new();
    }
}
