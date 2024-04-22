using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Temp;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class GetQuizResultBodyResponse
    {
        public List<QuestionBase> Questions { get; set; } //intebarile memorate de furnza(sunt in bd) //Placeholder -> rezutatul asta = null
        public List<QuestionResultBase> QuestionResults { get; set; } // question restulurile (din bd)

        public GetQuizResultBodyResponse(List<QuestionBase> questions, List<QuestionResultBase> questionResults)
        {
            Questions = questions;  
            QuestionResults = questionResults;
        }
    }
}
