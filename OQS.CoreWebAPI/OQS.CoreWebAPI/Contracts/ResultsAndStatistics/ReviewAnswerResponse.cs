using OQS.CoreWebAPI.Entities.ResultsAndStatistics;
using OQS.CoreWebAPI.Entities.ResultsAndStatistics.QuestionResults;

namespace OQS.CoreWebAPI.Contracts.ResultsAndStatistics
{
    public class ReviewAnswerResponse
    {
        public QuizResultHeader UpdatedQuizResultHeader { get; set; }
        public ReviewNeededQuestionResult UpdatedQuestionResult { get; set; }
    }
}
