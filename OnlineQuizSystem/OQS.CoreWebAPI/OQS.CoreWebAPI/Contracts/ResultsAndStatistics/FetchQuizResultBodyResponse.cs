using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;
using OQS.CoreWebAPI.Temp;

namespace OQS.CoreWebAPI.Contracts.ResultsAndStatistics
{
    public class FetchQuizResultBodyResponse
    {
        public List<QuestionBase> Questions { get; set; } = new();
        public List<QuestionResultBase> QuestionResults { get; set; } = new();
    }
}
