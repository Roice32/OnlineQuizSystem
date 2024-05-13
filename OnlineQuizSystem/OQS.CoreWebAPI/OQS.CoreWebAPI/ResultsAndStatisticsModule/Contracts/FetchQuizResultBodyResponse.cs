using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class FetchQuizResultBodyResponse
    {
        public List<QuestionBase> Questions { get; set; } = new();
        public List<QuestionResultBase> QuestionResults { get; set; } = new();
    }
}
