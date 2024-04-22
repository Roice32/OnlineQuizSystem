using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class GetQuizResultResponse
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public int CompletionTime { get; set; }
        public float Score { get; set; }
        public bool ReviewPending { get; set; }
        public string UserName { get; set; }
        public string QuizName { get; set; }
        public List<QuestionBase> Questions { get; set; } //intebarile memorate de furnza(sunt in bd) //Placeholder -> rezutatul asta = null
        public List<QuestionResultBase> QuestionResults { get; set; } // question restulurile (din bd)
    }
}
