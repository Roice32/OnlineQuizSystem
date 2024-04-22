using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities;
using OQS.CoreWebAPI.ResultsAndStatisticsModule.Entities.QuestionResults;

namespace OQS.CoreWebAPI.ResultsAndStatisticsModule.Contracts
{
    public class ReviewAnswerResponse
    {
        public QuizResultHeader UpdatedQuizResultHeader { get; set; }
        public QuestionResultBase UpdatedQuestionResult { get; set; }

        public ReviewAnswerResponse(QuizResultHeader updatedQuizResultHeader, QuestionResultBase updatedQuestionResult)
        {
            UpdatedQuizResultHeader = updatedQuizResultHeader;
            UpdatedQuestionResult = updatedQuestionResult;
        }
    }
}

