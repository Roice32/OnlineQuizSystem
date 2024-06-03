using OQS.CoreWebAPI.Contracts.CRUD;

namespace OQS.CoreWebAPI.Contracts.ResultsAndStatistics
{
    public class FetchQuizResultBodyResponse
    {
        public List<QuestionResponse> Questions { get; set; } = new();
        public List<QuestionResultResponse> QuestionResults { get; set; } = new();
    }
}
