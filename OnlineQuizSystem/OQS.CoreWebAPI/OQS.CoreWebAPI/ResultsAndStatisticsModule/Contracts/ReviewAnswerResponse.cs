using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class ReviewAnswerResponse
    {
        public QuizResultHeader UpdatedQuizResultHeader { get; set; }
        public ReviewNeededQuestionResult UpdatedQuestionResult { get; set; }
    }
}
